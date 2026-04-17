using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class GLJournalEntryLine
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid JournalEntryId { get; set; }
        [ForeignKey("JournalEntryId")]
        public GLJournalEntry? JournalEntry { get; set; }
        [Required]
        public Guid GLAccountId { get; set; }
        [ForeignKey("GLAccountId")]
        public GLAccount? GLAccount { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal DebitAmount { get; set; } = 0;
        [Column(TypeName = "decimal(15,2)")]
        public decimal CreditAmount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
