using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class Expense
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? ShopId { get; set; }
        [ForeignKey("ShopId")]
        public Shop? Shop { get; set; }
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(10,2)")]
        [Required]
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "RWF";
        [Required]
        public DateTime ExpenseDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? VendorName { get; set; }
        public string? ReceiptUrl { get; set; }
        public Guid? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? CreatedByUser { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
