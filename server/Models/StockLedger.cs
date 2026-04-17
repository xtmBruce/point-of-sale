using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class StockLedger
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public RawMaterial? Material { get; set; }
        [Required]
        public string TransactionType { get; set; } = string.Empty;
        [Column(TypeName = "decimal(10,4)")]
        [Required]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal? UnitCost { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? TotalValue { get; set; }
        public string? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
        public string? Notes { get; set; }
        public Guid? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? CreatedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
