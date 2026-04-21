using System.Text.Json;

namespace SmartPOS.API.DTOs
{
    public class SendNotificationDto
    {
        public string Recipient { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = "email";
        public Guid? CustomerId { get; set; }
        public Guid? CampaignId { get; set; }
    }

    public class TemplateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "email";
        public string? Subject { get; set; }
        public string Content { get; set; } = string.Empty;
        public JsonElement? Variables { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CampaignDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? TemplateId { get; set; }
        public string CampaignType { get; set; } = "email";
        public string? TargetAudience { get; set; }
        public JsonElement? Filters { get; set; }
        public DateTime? ScheduledAt { get; set; }
    }
}
