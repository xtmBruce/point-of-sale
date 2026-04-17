using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class CustomerNotificationPreference
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }
        public bool SmsEnabled { get; set; } = true;
        public bool EmailEnabled { get; set; } = true;
        public bool PushEnabled { get; set; } = true;
        public bool MarketingSms { get; set; } = true;
        public bool MarketingEmail { get; set; } = true;
        public bool MarketingPush { get; set; } = true;
        public bool LoyaltyNotifications { get; set; } = true;
        public bool PaymentReminders { get; set; } = true;
        public bool OrderUpdates { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
