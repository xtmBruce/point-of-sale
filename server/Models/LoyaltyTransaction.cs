using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class LoyaltyTransaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public int Points { get; set; }
        public string? Description { get; set; }
        public Guid? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
