using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class GoodsReceiptItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid GoodsReceiptId { get; set; }
        [ForeignKey("GoodsReceiptId")]
        public GoodsReceipt? GoodsReceipt { get; set; }
        public Guid MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public RawMaterial? Material { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal QuantityReceived { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal? UnitCost { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? TotalCost { get; set; }
        public string? BatchNumber { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public string QualityStatus { get; set; } = "good";
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
