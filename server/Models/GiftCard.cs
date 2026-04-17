using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class GiftCard
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string CardNumber { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal InitialValue { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal CurrentBalance { get; set; }
        public string Status { get; set; } = "active"; // active, inactive, expired, redeemed
        public Guid? PurchasedBy { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        [ForeignKey("PurchaseOrderId")]
        public Order? PurchaseOrder { get; set; }
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public Guid CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? Creator { get; set; }
        public string? Notes { get; set; }
        public bool IsDigital { get; set; } = false;
        public string? RecipientEmail { get; set; }
        public string? RecipientName { get; set; }
        public string? PurchaserName { get; set; }
        public string? Message { get; set; }
        public Guid? TemplateId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<GiftCardTransaction> Transactions { get; set; } = new List<GiftCardTransaction>();
    }
}
