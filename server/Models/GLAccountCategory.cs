using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class GLAccountCategory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public string AccountType { get; set; } = string.Empty; // "asset", "liability", "equity", "revenue", "expense"
        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public GLAccountCategory? Parent { get; set; }
        public int Level { get; set; } = 0;
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GLAccount> GLAccounts { get; set; } = new List<GLAccount>();
    }
}
