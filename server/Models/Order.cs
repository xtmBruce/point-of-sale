using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }
        public Guid? ShopId { get; set; }
        [ForeignKey("ShopId")]
        public Shop? Shop { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string OrderType { get; set; } = "regular";
        public string Status { get; set; } = "completed";
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxAmount { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountPaid { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")]
        public decimal RemainingAmount { get; set; } = 0;
        public string Currency { get; set; } = "RWF";
        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; } = 0;
        public string? PaymentMethod { get; set; }
        public string PaymentStatus { get; set; } = "pending";
        public string? Notes { get; set; }
        public Guid? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? Creator { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
