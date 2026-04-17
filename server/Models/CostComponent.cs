using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class CostComponent
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid BatchId { get; set; }
        [ForeignKey("BatchId")]
        public BottlingBatch? Batch { get; set; }
        [Required]
        public string ComponentType { get; set; } = string.Empty;
        [Required]
        public string ComponentName { get; set; } = string.Empty;
        [Column(TypeName = "decimal(10,4)")]
        [Required]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        [Required]
        public decimal UnitCost { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        [Required]
        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
