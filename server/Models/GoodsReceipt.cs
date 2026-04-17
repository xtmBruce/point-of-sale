using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class GoodsReceipt
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string GRNNumber { get; set; } = string.Empty;
        public Guid? PurchaseOrderId { get; set; }
        [ForeignKey("PurchaseOrderId")]
        public PurchaseOrder? PurchaseOrder { get; set; }
        [Required]
        public Guid SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }
        [Required]
        public DateTime ReceiptDate { get; set; }
        [Required]
        public Guid ReceivedBy { get; set; }
        [ForeignKey("ReceivedBy")]
        public User? ReceivedByUser { get; set; }
        public string Status { get; set; } = "received"; // "received", "inspected", "accepted", "rejected"
        [Column(TypeName = "decimal(15,2)")]
        public decimal? TotalValue { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GoodsReceiptItem> Items { get; set; } = new List<GoodsReceiptItem>();
    }
}
