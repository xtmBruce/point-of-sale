using System.Text.Json;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SmartPOS.API.Data;
using SmartPOS.API.DTOs;
using SmartPOS.API.Models;

namespace SmartPOS.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(AppDbContext db, IConfiguration config, ILogger<NotificationService> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }

        public async Task<NotificationSendResult> SendDirectAsync(SendNotificationDto dto, CancellationToken cancellationToken = default)
        {
            var channel = string.IsNullOrWhiteSpace(dto.Type) ? "email" : dto.Type.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(dto.Recipient))
            {
                return new NotificationSendResult
                {
                    Success = false,
                    Error = "Recipient is required"
                };
            }

            if (dto.CustomerId.HasValue)
            {
                var customerExists = await _db.Customers.AnyAsync(c => c.Id == dto.CustomerId.Value, cancellationToken);
                if (!customerExists)
                {
                    return new NotificationSendResult
                    {
                        Success = false,
                        Error = "Customer not found"
                    };
                }

                if (channel == "email")
                {
                    var customer = await _db.Customers.FirstAsync(c => c.Id == dto.CustomerId.Value, cancellationToken);
                    if (IsDefaultCustomer(customer))
                    {
                        return new NotificationSendResult
                        {
                            Success = false,
                            Error = "Email notifications are not sent to the default walk-in customer"
                        };
                    }
                }
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Type = string.IsNullOrWhiteSpace(dto.Type) ? "email" : dto.Type.ToLowerInvariant(),
                Subject = dto.Subject,
                Content = dto.Content,
                Recipient = dto.Recipient,
                CustomerId = dto.CustomerId,
                CampaignId = dto.CampaignId,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync(cancellationToken);

            await DeliverNotificationAsync(notification, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return new NotificationSendResult
            {
                Success = notification.Status == "sent",
                NotificationId = notification.Id,
                Error = notification.ErrorMessage
            };
        }

        public async Task<CampaignSendResult> SendCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default)
        {
            var campaign = await _db.NotificationCampaigns
                .Include(c => c.Template)
                .FirstOrDefaultAsync(c => c.Id == campaignId, cancellationToken);

            if (campaign == null)
            {
                return new CampaignSendResult
                {
                    Success = false,
                    CampaignId = campaignId,
                    Error = "Campaign not found"
                };
            }

            if (campaign.Template == null || !campaign.Template.IsActive)
            {
                return new CampaignSendResult
                {
                    Success = false,
                    CampaignId = campaignId,
                    Error = "Campaign template is missing or inactive"
                };
            }

            if (campaign.Status == "sending")
            {
                return new CampaignSendResult
                {
                    Success = false,
                    CampaignId = campaignId,
                    Error = "Campaign is already being sent"
                };
            }

            campaign.Status = "sending";
            campaign.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);

            var recipients = await GetTargetCustomersAsync(campaign, cancellationToken);
            var template = campaign.Template;
            var channel = string.IsNullOrWhiteSpace(template.Type) ? "email" : template.Type.ToLowerInvariant();

            var notifications = new List<Notification>();
            foreach (var customer in recipients)
            {
                var recipient = ResolveRecipient(channel, customer);
                if (string.IsNullOrWhiteSpace(recipient))
                {
                    continue;
                }

                notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    CampaignId = campaign.Id,
                    CustomerId = customer.Id,
                    Type = channel,
                    Subject = RenderTemplate(template.Subject, customer),
                    Content = RenderTemplate(template.Content, customer) ?? string.Empty,
                    Recipient = recipient,
                    Status = "pending",
                    CreatedAt = DateTime.UtcNow
                });
            }

            _db.Notifications.AddRange(notifications);
            await _db.SaveChangesAsync(cancellationToken);

            foreach (var notification in notifications)
            {
                await DeliverNotificationAsync(notification, cancellationToken);
            }

            var sentCount = notifications.Count(n => n.Status == "sent");
            var failedCount = notifications.Count - sentCount;

            campaign.TotalRecipients = notifications.Count;
            campaign.SentCount = sentCount;
            campaign.SentAt = sentCount > 0 ? DateTime.UtcNow : null;
            campaign.UpdatedAt = DateTime.UtcNow;

            if (notifications.Count == 0)
            {
                campaign.Status = "failed";
            }
            else if (failedCount == 0)
            {
                campaign.Status = "sent";
            }
            else if (sentCount == 0)
            {
                campaign.Status = "failed";
            }
            else
            {
                campaign.Status = "partial";
            }

            await _db.SaveChangesAsync(cancellationToken);

            return new CampaignSendResult
            {
                Success = sentCount > 0,
                CampaignId = campaign.Id,
                TotalRecipients = notifications.Count,
                SentCount = sentCount,
                FailedCount = failedCount,
                Error = notifications.Count == 0
                    ? "No eligible recipients found"
                    : sentCount > 0
                        ? null
                        : "All recipient deliveries failed"
            };
        }

        public async Task<int> ProcessScheduledCampaignsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var campaigns = await _db.NotificationCampaigns
                .Where(c => c.Status == "scheduled" && c.ScheduledAt.HasValue && c.ScheduledAt <= now)
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            var processed = 0;
            foreach (var campaignId in campaigns)
            {
                var result = await SendCampaignAsync(campaignId, cancellationToken);
                if (result.Success)
                {
                    processed++;
                }
            }

            return processed;
        }

        public async Task<int> ProcessBirthdayWishesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var today = DateOnly.FromDateTime(now);
            var dayStartUtc = now.Date;
            var nextDayStartUtc = dayStartUtc.AddDays(1);
            const string birthdaySubject = "Happy Birthday from aucaPOS";
            const string birthdayBodyTemplate = "Happy Birthday {first_name}! We wish you a wonderful day from aucaPOS.";

            var birthdayCustomers = await _db.Customers
                .Where(c =>
                    c.IsActive &&
                    c.Birthday.HasValue &&
                    c.Birthday.Value.Month == today.Month &&
                    c.Birthday.Value.Day == today.Day)
                .ToListAsync(cancellationToken);

            var eligibleCustomers = birthdayCustomers
                .Where(c => !IsDefaultCustomer(c))
                .Where(c => !string.IsNullOrWhiteSpace(c.Email))
                .ToList();

            if (eligibleCustomers.Count == 0)
            {
                return 0;
            }

            var eligibleCustomerIds = eligibleCustomers.Select(c => c.Id).ToList();
            var alreadySentIds = await _db.Notifications
                .Where(n =>
                    n.Type == "email" &&
                    n.Subject == birthdaySubject &&
                    n.CustomerId.HasValue &&
                    eligibleCustomerIds.Contains(n.CustomerId.Value) &&
                    n.CreatedAt >= dayStartUtc &&
                    n.CreatedAt < nextDayStartUtc)
                .Select(n => n.CustomerId!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);

            var alreadySentSet = alreadySentIds.ToHashSet();
            var notifications = new List<Notification>();

            foreach (var customer in eligibleCustomers)
            {
                if (alreadySentSet.Contains(customer.Id))
                {
                    continue;
                }

                notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    Type = "email",
                    Subject = birthdaySubject,
                    Content = RenderBirthdayMessage(birthdayBodyTemplate, customer),
                    Recipient = customer.Email!,
                    Status = "pending",
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (notifications.Count == 0)
            {
                return 0;
            }

            _db.Notifications.AddRange(notifications);
            await _db.SaveChangesAsync(cancellationToken);

            foreach (var notification in notifications)
            {
                await DeliverNotificationAsync(notification, cancellationToken);
            }

            await _db.SaveChangesAsync(cancellationToken);
            return notifications.Count(n => n.Status == "sent");
        }

        private async Task<List<Customer>> GetTargetCustomersAsync(NotificationCampaign campaign, CancellationToken cancellationToken)
        {
            var channel = campaign.Template?.Type?.ToLowerInvariant() ?? "email";
            var customers = await _db.Customers
                .Where(c => c.IsActive)
                .ToListAsync(cancellationToken);

            if (channel == "email")
            {
                customers = customers.Where(customer => !IsDefaultCustomer(customer)).ToList();
            }

            if (customers.Count == 0)
            {
                return customers;
            }

            var customerIds = customers.Select(c => c.Id).ToList();
            var preferenceMap = await _db.CustomerNotificationPreferences
                .Where(p => customerIds.Contains(p.CustomerId))
                .ToDictionaryAsync(p => p.CustomerId, p => p, cancellationToken);

            var filtered = customers.Where(customer =>
            {
                preferenceMap.TryGetValue(customer.Id, out var pref);
                return IsChannelEnabled(pref, channel) && IsCampaignCategoryEnabled(pref, channel, campaign.CampaignType);
            }).ToList();

            filtered = ApplyAudienceFilter(filtered, campaign.TargetAudience);
            filtered = ApplyJsonFilters(filtered, campaign.Filters);

            return filtered;
        }

        private static bool IsChannelEnabled(CustomerNotificationPreference? pref, string channel)
        {
            if (pref == null)
            {
                return true;
            }

            return channel switch
            {
                "sms" => pref.SmsEnabled,
                "push" => pref.PushEnabled,
                _ => pref.EmailEnabled
            };
        }

        private static bool IsCampaignCategoryEnabled(CustomerNotificationPreference? pref, string channel, string? campaignType)
        {
            if (pref == null || string.IsNullOrWhiteSpace(campaignType))
            {
                return true;
            }

            var normalizedType = campaignType.ToLowerInvariant();
            return normalizedType switch
            {
                "promotion" => channel switch
                {
                    "sms" => pref.MarketingSms,
                    "push" => pref.MarketingPush,
                    _ => pref.MarketingEmail
                },
                "loyalty" => pref.LoyaltyNotifications,
                "payment_reminder" => pref.PaymentReminders,
                _ => true
            };
        }

        private static List<Customer> ApplyAudienceFilter(List<Customer> customers, string? targetAudience)
        {
            if (string.IsNullOrWhiteSpace(targetAudience) || targetAudience == "all")
            {
                return customers;
            }

            var now = DateTime.UtcNow;
            return targetAudience.ToLowerInvariant() switch
            {
                "new_customers" => customers.Where(c => c.CreatedAt >= now.AddDays(-30)).ToList(),
                "returning_customers" => customers.Where(c => c.TotalSpent > 0).ToList(),
                "vip_customers" => customers.Where(c => c.TotalSpent >= 1000 || c.LoyaltyTier == "gold" || c.LoyaltyTier == "platinum").ToList(),
                _ => customers
            };
        }

        private static List<Customer> ApplyJsonFilters(List<Customer> customers, string? filtersJson)
        {
            if (string.IsNullOrWhiteSpace(filtersJson))
            {
                return customers;
            }

            try
            {
                using var doc = JsonDocument.Parse(filtersJson);
                var root = doc.RootElement;

                if (root.ValueKind != JsonValueKind.Object)
                {
                    return customers;
                }

                var filtered = customers;

                if (root.TryGetProperty("city", out var cityEl) && cityEl.ValueKind == JsonValueKind.String)
                {
                    var city = cityEl.GetString();
                    if (!string.IsNullOrWhiteSpace(city))
                    {
                        filtered = filtered.Where(c => string.Equals(c.City, city, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                }

                if (root.TryGetProperty("country", out var countryEl) && countryEl.ValueKind == JsonValueKind.String)
                {
                    var country = countryEl.GetString();
                    if (!string.IsNullOrWhiteSpace(country))
                    {
                        filtered = filtered.Where(c => string.Equals(c.Country, country, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                }

                if (root.TryGetProperty("loyalty_tier", out var tierEl) && tierEl.ValueKind == JsonValueKind.String)
                {
                    var tier = tierEl.GetString();
                    if (!string.IsNullOrWhiteSpace(tier))
                    {
                        filtered = filtered.Where(c => string.Equals(c.LoyaltyTier, tier, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                }

                if (root.TryGetProperty("min_total_spent", out var minSpentEl) && minSpentEl.ValueKind == JsonValueKind.Number && minSpentEl.TryGetDecimal(out var minSpent))
                {
                    filtered = filtered.Where(c => c.TotalSpent >= minSpent).ToList();
                }

                if (root.TryGetProperty("customer_ids", out var idsEl) && idsEl.ValueKind == JsonValueKind.Array)
                {
                    var ids = new HashSet<Guid>();
                    foreach (var idEl in idsEl.EnumerateArray())
                    {
                        if (idEl.ValueKind == JsonValueKind.String && Guid.TryParse(idEl.GetString(), out var id))
                        {
                            ids.Add(id);
                        }
                    }

                    if (ids.Count > 0)
                    {
                        filtered = filtered.Where(c => ids.Contains(c.Id)).ToList();
                    }
                }

                return filtered;
            }
            catch
            {
                return customers;
            }
        }

        private static string ResolveRecipient(string channel, Customer customer)
        {
            return channel switch
            {
                "sms" => customer.Phone ?? string.Empty,
                "push" => customer.Id.ToString(),
                _ => customer.Email ?? string.Empty
            };
        }

        private static string? RenderTemplate(string? text, Customer customer)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            return text
                .Replace("{customer_name}", $"{customer.FirstName} {customer.LastName}", StringComparison.OrdinalIgnoreCase)
                .Replace("{{customer_name}}", $"{customer.FirstName} {customer.LastName}", StringComparison.OrdinalIgnoreCase)
                .Replace("{first_name}", customer.FirstName, StringComparison.OrdinalIgnoreCase)
                .Replace("{last_name}", customer.LastName, StringComparison.OrdinalIgnoreCase)
                .Replace("{{first_name}}", customer.FirstName, StringComparison.OrdinalIgnoreCase)
                .Replace("{{last_name}}", customer.LastName, StringComparison.OrdinalIgnoreCase);
        }

        private static string RenderBirthdayMessage(string template, Customer customer)
        {
            return template
                .Replace("{customer_name}", $"{customer.FirstName} {customer.LastName}", StringComparison.OrdinalIgnoreCase)
                .Replace("{{customer_name}}", $"{customer.FirstName} {customer.LastName}", StringComparison.OrdinalIgnoreCase)
                .Replace("{first_name}", customer.FirstName, StringComparison.OrdinalIgnoreCase)
                .Replace("{last_name}", customer.LastName, StringComparison.OrdinalIgnoreCase)
                .Replace("{{first_name}}", customer.FirstName, StringComparison.OrdinalIgnoreCase)
                .Replace("{{last_name}}", customer.LastName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsDefaultCustomer(Customer customer)
        {
            var firstName = customer.FirstName?.Trim().ToLowerInvariant();
            var lastName = customer.LastName?.Trim().ToLowerInvariant();

            return firstName == "walk-in" && lastName == "customer";
        }

        private async Task DeliverNotificationAsync(Notification notification, CancellationToken cancellationToken)
        {
            if (notification.Type == "email")
            {
                var success = await SendEmailAsync(notification.Recipient, notification.Subject ?? "Notification", notification.Content, cancellationToken);
                notification.Status = success ? "sent" : "failed";
                notification.SentAt = success ? DateTime.UtcNow : null;
                notification.ErrorMessage = success ? null : GetEmailFailureReason();
                return;
            }

            // SMS/push are currently tracked in DB; delivery is simulated.
            notification.Status = "sent";
            notification.SentAt = DateTime.UtcNow;
            notification.ErrorMessage = null;
        }

        private async Task<bool> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken)
        {
            try
            {
                var emailSection = _config.GetSection("Email");
                var host = emailSection.GetValue<string>("Host");
                var port = emailSection.GetValue<int>("Port");
                var username = emailSection.GetValue<string>("Username");
                var password = emailSection.GetValue<string>("Password");
                var fromName = emailSection.GetValue<string>("FromName");
                var fromEmail = emailSection.GetValue<string>("FromEmail");

                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fromEmail))
                {
                    _logger.LogWarning("Email settings are incomplete. Skipping send.");
                    return false;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName ?? "SmartPOS", fromEmail));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                using var client = new SmtpClient();
                await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
                await client.AuthenticateAsync(username, password, cancellationToken);
                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification to {Recipient}", to);
                return false;
            }
        }

        private string GetEmailFailureReason()
        {
            var emailSection = _config.GetSection("Email");
            var host = emailSection.GetValue<string>("Host");
            var username = emailSection.GetValue<string>("Username");
            var password = emailSection.GetValue<string>("Password");
            var fromEmail = emailSection.GetValue<string>("FromEmail");

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fromEmail))
            {
                return "Email settings are incomplete";
            }

            return "Failed to send email";
        }
    }
}
