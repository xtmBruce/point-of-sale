using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class BottleSize
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int SizeMl { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal BottleCost { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal LabelCost { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")]
        public decimal PackagingCost { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")]
        public decimal LaborCost { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
