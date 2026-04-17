using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class GLAccount
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string AccountCode { get; set; } = string.Empty;
        [Required]
        public string AccountName { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public GLAccountCategory? Category { get; set; }
        [Required]
        public string AccountType { get; set; } = string.Empty; // "asset", "liability", "equity", "revenue", "expense"
        [Required]
        public string NormalBalance { get; set; } = string.Empty; // "debit" or "credit"
        public bool IsContra { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GLJournalEntryLine> JournalEntryLines { get; set; } = new List<GLJournalEntryLine>();
    }
}
