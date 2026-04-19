using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class GLJournalEntry
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string EntryNumber { get; set; } = string.Empty;
        public DateOnly EntryDate { get; set; }
        public string? Description { get; set; }
        public string? ReferenceNumber { get; set; }
        public string Status { get; set; } = "draft";
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalDebits { get; set; } = 0;
        [Column(TypeName = "decimal(15,2)")]
        public decimal TotalCredits { get; set; } = 0;
        public Guid? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? Creator { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
