using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class RecipeMaterial
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid RecipeId { get; set; }
        [ForeignKey("RecipeId")]
        public BottlingRecipe? Recipe { get; set; }
        [Required]
        public Guid MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public RawMaterial? Material { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        [Required]
        public decimal QuantityPerUnit { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
