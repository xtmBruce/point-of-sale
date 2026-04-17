using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class NotificationTrigger
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string TriggerType { get; set; } = string.Empty; // "promotion", "loyalty", "payment_reminder", "order_update", "custom"
        public string? Conditions { get; set; }
        public Guid? TemplateId { get; set; }
        [ForeignKey("TemplateId")]
        public NotificationTemplate? Template { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
