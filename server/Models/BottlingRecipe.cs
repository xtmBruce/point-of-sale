using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class BottlingRecipe
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? BottleSizeId { get; set; }
        [ForeignKey("BottleSizeId")]
        public BottleSize? BottleSize { get; set; }
        public string Version { get; set; } = "1.0";
        public string Status { get; set; } = "active";
        public string? Category { get; set; }
        public string? DifficultyLevel { get; set; }
        public int? EstimatedProductionTime { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? TargetCost { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal? MarkupPercentage { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? SellingPrice { get; set; }
        public string Currency { get; set; } = "RWF";
        public string? QualityStandards { get; set; }
        public int? ShelfLifeDays { get; set; }
        public int? BatchSizeMin { get; set; }
        public int? BatchSizeMax { get; set; }
        public string? ProductionNotes { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal? YieldPercentage { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal? WastePercentage { get; set; }
        public string? RecipeImageUrl { get; set; }
        public string? InstructionManualUrl { get; set; }
        public string? SafetyInstructions { get; set; }
        public string? TestingRequirements { get; set; }
        public string? StorageRequirements { get; set; }
        public string? QualityCheckpoints { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? EfficiencyRating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
