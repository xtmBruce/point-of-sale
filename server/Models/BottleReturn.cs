using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class BottleReturn
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }
        public int Quantity { get; set; }
        public string? BottleSize { get; set; }
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
        public Guid? ProcessedBy { get; set; }
        [ForeignKey("ProcessedBy")]
        public User? Processor { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
