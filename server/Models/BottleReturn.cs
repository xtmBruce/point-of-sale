using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class BottleReturn
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string? BottleSize { get; set; }
        [Required]
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
        public Guid? ProcessedBy { get; set; }
        [ForeignKey("ProcessedBy")]
        public User? ProcessedByUser { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountApplied { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
