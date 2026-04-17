using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class BottlingBatch
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string BatchNumber { get; set; } = string.Empty;
        [Required]
        public Guid RecipeId { get; set; }
        [ForeignKey("RecipeId")]
        public BottlingRecipe? Recipe { get; set; }
        public Guid? BulkPerfumeId { get; set; }
        [ForeignKey("BulkPerfumeId")]
        public PerfumeBulk? BulkPerfume { get; set; }
        [Required]
        public int QuantityPlanned { get; set; }
        public int QuantityProduced { get; set; } = 0;
        public int QuantityDefective { get; set; } = 0;
        public string Status { get; set; } = "planned"; // "planned", "in_production", "completed", "cancelled"
        public DateTime? ProductionDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? UnitCost { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? TotalCost { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal ProfitMargin { get; set; } = 50.00m;
        [Column(TypeName = "decimal(10,2)")]
        public decimal? SellingPrice { get; set; }
        public Guid? OperatorId { get; set; }
        [ForeignKey("OperatorId")]
        public User? Operator { get; set; }
        public Guid? SupervisorId { get; set; }
        [ForeignKey("SupervisorId")]
        public User? Supervisor { get; set; }
        public string? Notes { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? QualityScore { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? EfficiencyRating { get; set; }
        public Guid? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? CreatedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CostComponent> CostComponents { get; set; } = new List<CostComponent>();
    }
}
