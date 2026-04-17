using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class GiftCardTransaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid GiftCardId { get; set; }
        [ForeignKey("GiftCardId")]
        public GiftCard? GiftCard { get; set; }
        [Required]
        public string TransactionType { get; set; } = string.Empty; // purchase, redemption, reload, refund, adjustment
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal BalanceBefore { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal BalanceAfter { get; set; }
        public Guid? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        public Guid ProcessedBy { get; set; }
        [ForeignKey("ProcessedBy")]
        public User? Processor { get; set; }
        public Guid? ShopId { get; set; }
        [ForeignKey("ShopId")]
        public Shop? Shop { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
