using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class PurchaseOrderItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PurchaseOrderId { get; set; }
        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder? PurchaseOrder { get; set; }
        public Guid? MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public RawMaterial? Material { get; set; }
        public string? ItemName { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal QuantityOrdered { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal QuantityReceived { get; set; } = 0;
        [Column(TypeName = "decimal(10,4)")]
        public decimal UnitCost { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalCost { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
