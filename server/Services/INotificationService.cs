using SmartPOS.API.DTOs;

namespace SmartPOS.API.Services
{
    public sealed class NotificationSendResult
    {
        public bool Success { get; set; }
        public Guid NotificationId { get; set; }
        public string? Error { get; set; }
    }

    public sealed class CampaignSendResult
    {
        public bool Success { get; set; }
        public Guid CampaignId { get; set; }
        public int TotalRecipients { get; set; }
        public int SentCount { get; set; }
        public int FailedCount { get; set; }
        public string? Error { get; set; }
    }

    public interface INotificationService
    {
        Task<NotificationSendResult> SendDirectAsync(SendNotificationDto dto, CancellationToken cancellationToken = default);
        Task<CampaignSendResult> SendCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default);
        Task<int> ProcessScheduledCampaignsAsync(CancellationToken cancellationToken = default);
    }
}
