using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        public Guid? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = "RWF";
        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercent { get; set; } = 0;
        public string? ProductName { get; set; }
        public string? ProductType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
