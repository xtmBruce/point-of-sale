using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Sku { get; set; }
        public string? Barcode { get; set; }
        public Guid? BrandId { get; set; }
        [ForeignKey("BrandId")]
        public Brand? Brand { get; set; }
        public Guid? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        public string ProductType { get; set; } = "general";
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Variant { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? CostPrice { get; set; }
        public string Currency { get; set; } = "RWF";
        public int StockQuantity { get; set; } = 0;
        public int CurrentStock { get; set; } = 0;
        public int ReservedStock { get; set; } = 0;
        public int AvailableStock { get; set; } = 0;
        public int MinStockLevel { get; set; } = 0;
        public int? MaxStockLevel { get; set; }
        public int ReorderPoint { get; set; } = 20;
        public string Unit { get; set; } = "piece";
        [Column(TypeName = "decimal(8,3)")]
        public decimal? Weight { get; set; }
        public string? Dimensions { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}