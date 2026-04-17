using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class PerfumeBottling
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid BulkPerfumeId { get; set; }
        [ForeignKey("BulkPerfumeId")]
        public PerfumeBulk? BulkPerfume { get; set; }
        [Required]
        public Guid BottleSizeId { get; set; }
        [ForeignKey("BottleSizeId")]
        public BottleSizes? BottleSize { get; set; }
        [Required]
        public int QuantityBottled { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        [Required]
        public decimal TotalCost { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal? SellingPricePerMl { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? CreatedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
