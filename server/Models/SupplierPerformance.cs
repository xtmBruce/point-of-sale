using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class SupplierPerformance
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }
        public DateOnly PerformanceDate { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? DeliveryRating { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? QualityRating { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? PriceRating { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? ServiceRating { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? OverallRating { get; set; }
        public string? Notes { get; set; }
        public Guid? EvaluatedBy { get; set; }
        [ForeignKey("EvaluatedBy")]
        public User? EvaluatedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
