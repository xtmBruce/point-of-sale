using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? CampaignId { get; set; }
        [ForeignKey("CampaignId")]
        public NotificationCampaign? Campaign { get; set; }
        public Guid? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }
        [Required]
        public string Type { get; set; } = string.Empty; // "sms", "email", "push"
        public string? Subject { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;
        [Required]
        public string Recipient { get; set; } = string.Empty;
        public string Status { get; set; } = "pending"; // "pending", "sent", "delivered", "failed", "opened", "clicked"
        public DateTime? SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? OpenedAt { get; set; }
        public DateTime? ClickedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Metadata { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
