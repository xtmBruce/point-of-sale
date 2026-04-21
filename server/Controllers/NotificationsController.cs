using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPOS.API.Data;
using SmartPOS.API.Models;
using SmartPOS.API.DTOs;
using SmartPOS.API.Services;

namespace SmartPOS.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private static readonly TimeZoneInfo CentralAfricaTimeZone = ResolveCentralAfricaTimeZone();
        private readonly AppDbContext _db;
        private readonly INotificationService _notificationService;

        public NotificationsController(AppDbContext db, INotificationService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }

        // GET /api/notifications
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notifications = await _db.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(100)
                .ToListAsync();
            return Ok(notifications);
        }

        // POST /api/notifications/send-test - send email
        [HttpPost("send-test")]
        public async Task<IActionResult> SendTest([FromBody] SendNotificationDto dto)
        {
            var result = await _notificationService.SendDirectAsync(dto);
            if (!result.Success)
                return BadRequest(new { error = result.Error ?? "Failed to send notification" });

            return Ok(new { success = true, notification_id = result.NotificationId });
        }

        // GET /api/notifications/templates
        [HttpGet("templates")]
        public async Task<IActionResult> GetTemplates() =>
            Ok(new { templates = await _db.NotificationTemplates.Where(t => t.IsActive).ToListAsync() });

        // POST /api/notifications/templates
        [HttpPost("templates")]
        public async Task<IActionResult> CreateTemplate(TemplateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { error = "Name is required" });

            var template = new NotificationTemplate
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Type = dto.Type,
                Subject = dto.Subject,
                Content = dto.Content,
                Variables = dto.Variables.HasValue ? dto.Variables.Value.GetRawText() : null,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.NotificationTemplates.Add(template);
            await _db.SaveChangesAsync();
            return Ok(template);
        }

        // PUT /api/notifications/templates/{id}
        [HttpPut("templates/{id}")]
        public async Task<IActionResult> UpdateTemplate(Guid id, TemplateDto dto)
        {
            var template = await _db.NotificationTemplates.FindAsync(id);
            if (template == null) return NotFound();
            template.Name = dto.Name;
            template.Type = dto.Type;
            template.Subject = dto.Subject;
            template.Content = dto.Content;
            template.Variables = dto.Variables.HasValue ? dto.Variables.Value.GetRawText() : null;
            template.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(template);
        }

        // DELETE /api/notifications/templates/{id}
        [HttpDelete("templates/{id}")]
        public async Task<IActionResult> DeleteTemplate(Guid id)
        {
            var template = await _db.NotificationTemplates.FindAsync(id);
            if (template == null) return NotFound();
            template.IsActive = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // GET /api/notifications/campaigns
        [HttpGet("campaigns")]
        public async Task<IActionResult> GetCampaigns() =>
            Ok(new { campaigns = await _db.NotificationCampaigns.Include(c => c.Template).ToListAsync() });

        // POST /api/notifications/campaigns
        [HttpPost("campaigns")]
        public async Task<IActionResult> CreateCampaign(CampaignDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { error = "Name is required" });

            Guid? templateId = null;
            if (!string.IsNullOrWhiteSpace(dto.TemplateId) && Guid.TryParse(dto.TemplateId, out var parsedId))
                templateId = parsedId;

            var campaign = new NotificationCampaign
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                TemplateId = templateId,
                CampaignType = dto.CampaignType,
                TargetAudience = dto.TargetAudience,
                Filters = dto.Filters.HasValue ? dto.Filters.Value.GetRawText() : null,
                ScheduledAt = NormalizeScheduledAtToUtc(dto.ScheduledAt),
                Status = dto.ScheduledAt.HasValue ? "scheduled" : "draft",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.NotificationCampaigns.Add(campaign);
            await _db.SaveChangesAsync();
            return Ok(campaign);
        }

        // PUT /api/notifications/campaigns/{id}
        [HttpPut("campaigns/{id}")]
        public async Task<IActionResult> UpdateCampaign(Guid id, CampaignDto dto)
        {
            var campaign = await _db.NotificationCampaigns.FindAsync(id);
            if (campaign == null) return NotFound();
            Guid? templateId = null;
            if (!string.IsNullOrWhiteSpace(dto.TemplateId) && Guid.TryParse(dto.TemplateId, out var parsedId))
                templateId = parsedId;
            campaign.Name = dto.Name;
            campaign.Description = dto.Description;
            campaign.TemplateId = templateId;
            campaign.CampaignType = dto.CampaignType;
            campaign.TargetAudience = dto.TargetAudience;
            campaign.Filters = dto.Filters.HasValue ? dto.Filters.Value.GetRawText() : null;
            campaign.ScheduledAt = NormalizeScheduledAtToUtc(dto.ScheduledAt);
            campaign.Status = dto.ScheduledAt.HasValue && campaign.Status != "sent" ? "scheduled" : campaign.Status;
            campaign.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(campaign);
        }

        // POST /api/notifications/campaigns/{id}/send
        [HttpPost("campaigns/{id}/send")]
        public async Task<IActionResult> SendCampaign(Guid id)
        {
            var result = await _notificationService.SendCampaignAsync(id);
            if (!result.Success)
                return BadRequest(new { error = result.Error ?? "Failed to send campaign" });

            return Ok(new
            {
                success = true,
                campaign_id = result.CampaignId,
                total_recipients = result.TotalRecipients,
                sent_count = result.SentCount,
                failed_count = result.FailedCount
            });
        }

        // DELETE /api/notifications/campaigns/{id}
        [HttpDelete("campaigns/{id}")]
        public async Task<IActionResult> DeleteCampaign(Guid id)
        {
            var campaign = await _db.NotificationCampaigns.FindAsync(id);
            if (campaign == null) return NotFound();
            _db.NotificationCampaigns.Remove(campaign);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // GET /api/notifications/analytics
        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics()
        {
            var total = await _db.Notifications.CountAsync();
            var sent = await _db.Notifications.CountAsync(n => n.Status == "sent");
            var failed = await _db.Notifications.CountAsync(n => n.Status == "failed");
            var pending = await _db.Notifications.CountAsync(n => n.Status == "pending");

            return Ok(new { total, sent, failed, pending });
        }

        // GET /api/notifications/triggers
        [HttpGet("triggers")]
        public async Task<IActionResult> GetTriggers() =>
            Ok(new { triggers = await _db.NotificationTriggers.ToListAsync() });

        private static DateTime? NormalizeScheduledAtToUtc(DateTime? scheduledAt)
        {
            if (!scheduledAt.HasValue)
            {
                return null;
            }

            var value = scheduledAt.Value;
            if (value.Kind == DateTimeKind.Utc)
            {
                return value;
            }

            if (value.Kind == DateTimeKind.Local)
            {
                return value.ToUniversalTime();
            }

            // Treat browser datetime-local values as Central Africa local time.
            var unspecified = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, CentralAfricaTimeZone);
        }

        private static TimeZoneInfo ResolveCentralAfricaTimeZone()
        {
            var candidateIds = new[]
            {
                "South Africa Standard Time", // Windows
                "Africa/Harare",              // IANA
                "Africa/Blantyre"             // IANA
            };

            foreach (var id in candidateIds)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch (TimeZoneNotFoundException)
                {
                }
                catch (InvalidTimeZoneException)
                {
                }
            }

            return TimeZoneInfo.Utc;
        }
    }
}
