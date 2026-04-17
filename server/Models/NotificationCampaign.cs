using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class NotificationCampaign
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? TemplateId { get; set; }
        [ForeignKey("TemplateId")]
        public NotificationTemplate? Template { get; set; }
        public string CampaignType { get; set; } = string.Empty;
        public string? TargetAudience { get; set; }
        public string? Filters { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public string Status { get; set; } = "draft";
        public int TotalRecipients { get; set; } = 0;
        public int SentCount { get; set; } = 0;
        public int OpenedCount { get; set; } = 0;
        public int ClickedCount { get; set; } = 0;
        public Guid? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? Creator { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
