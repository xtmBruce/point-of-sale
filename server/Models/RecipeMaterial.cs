using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class RecipeMaterial
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RecipeId { get; set; }
        [ForeignKey("RecipeId")]
        public BottlingRecipe? Recipe { get; set; }
        public Guid MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public RawMaterial? Material { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal QuantityPerUnit { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
