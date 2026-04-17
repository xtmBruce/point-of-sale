using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class StockTransfer
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TransferNumber { get; set; } = string.Empty;
        public string FromType { get; set; } = string.Empty;
        public Guid FromId { get; set; }
        public string ToType { get; set; } = string.Empty;
        public Guid ToId { get; set; }
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = "pending";
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
