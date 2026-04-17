using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class PriceChangeLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? OldPrice { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        [Required]
        public decimal NewPrice { get; set; }
        public string? ChangeReason { get; set; }
        public string? PricingStrategy { get; set; }
        public Guid? CalculatedBy { get; set; }
        [ForeignKey("CalculatedBy")]
        public User? CalculatedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
