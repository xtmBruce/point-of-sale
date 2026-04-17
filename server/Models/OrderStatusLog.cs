using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class OrderStatusLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        public string? StatusFrom { get; set; }
        [Required]
        public string StatusTo { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public Guid? ChangedBy { get; set; }
        [ForeignKey("ChangedBy")]
        public User? ChangedByUser { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
