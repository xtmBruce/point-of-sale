using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class CostComponent
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BatchId { get; set; }
        [ForeignKey("BatchId")]
        public BottlingBatch? Batch { get; set; }
        public string ComponentType { get; set; } = string.Empty;
        public string ComponentName { get; set; } = string.Empty;
        [Column(TypeName = "decimal(10,4)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal UnitCost { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
