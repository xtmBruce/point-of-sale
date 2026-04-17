using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class FinancialTransaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public DateTime TransactionDate { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(15,2)")]
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string TransactionType { get; set; } = string.Empty;
        public Guid? GLAccountId { get; set; }
        [ForeignKey("GLAccountId")]
        public GLAccount? GLAccount { get; set; }
        public string? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
        public Guid? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? CreatedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
