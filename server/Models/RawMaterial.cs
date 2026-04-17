using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class RawMaterial
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; } = string.Empty;
        [Required]
        public string Unit { get; set; } = string.Empty;
        [Column(TypeName = "decimal(10,2)")]
        public decimal CurrentStock { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")]
        public decimal MinStockLevel { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")]
        public decimal? MaxStockLevel { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal ReorderPoint { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")]
        public decimal SafetyStock { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")]
        [Required]
        public decimal CostPerUnit { get; set; }
        public Guid? SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierContact { get; set; }
        public int LeadTimeDays { get; set; } = 0;
        public int? ShelfLifeDays { get; set; }
        public string? StorageRequirements { get; set; }
        public string? QualityStandards { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RecipeMaterial> RecipeMaterials { get; set; } = new List<RecipeMaterial>();
        public ICollection<SupplierMaterial> SupplierMaterials { get; set; } = new List<SupplierMaterial>();
    }
}
