using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class StockTransfer
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string TransferNumber { get; set; } = string.Empty;
        [Required]
        public string FromType { get; set; } = string.Empty; // "shop" or "warehouse"
        [Required]
        public Guid FromId { get; set; }
        [Required]
        public string ToType { get; set; } = string.Empty; // "shop" or "warehouse"
        [Required]
        public Guid ToId { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string Status { get; set; } = "pending"; // "pending", "in_progress", "completed", "cancelled"
        public string? Notes { get; set; }
        public Guid? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? CreatedByUser { get; set; }
        public Guid? CompletedBy { get; set; }
        [ForeignKey("CompletedBy")]
        public User? CompletedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }
}
