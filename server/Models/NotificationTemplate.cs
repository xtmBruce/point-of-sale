using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class NotificationTemplate
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; } = string.Empty; // "sms", "email", "push"
        public string? Subject { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;
        public string? Variables { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<NotificationCampaign> NotificationCampaigns { get; set; } = new List<NotificationCampaign>();
        public ICollection<NotificationTrigger> NotificationTriggers { get; set; } = new List<NotificationTrigger>();
    }
}
