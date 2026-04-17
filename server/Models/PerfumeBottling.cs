using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class PerfumeBottling
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BulkPerfumeId { get; set; }
        [ForeignKey("BulkPerfumeId")]
        public PerfumeBulk? BulkPerfume { get; set; }
        public Guid BottleSizeId { get; set; }
        [ForeignKey("BottleSizeId")]
        public BottleSize? BottleSize { get; set; }
        public int QuantityBottled { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalCost { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal? SellingPricePerMl { get; set; }
        public Guid CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? Creator { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
