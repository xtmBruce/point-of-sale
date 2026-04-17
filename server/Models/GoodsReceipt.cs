using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class GoodsReceipt
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string GRNNumber { get; set; } = string.Empty;
        public Guid? PurchaseOrderId { get; set; }
        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder? PurchaseOrder { get; set; }
        public Guid SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }
        public DateOnly ReceiptDate { get; set; }
        public Guid ReceivedBy { get; set; }
        [ForeignKey("ReceivedBy")]
        public User? ReceivedByUser { get; set; }
        public string Status { get; set; } = "received";
        [Column(TypeName = "decimal(15,2)")]
        public decimal? TotalValue { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
