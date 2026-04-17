using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public Category? Parent { get; set; }
        public string? Path { get; set; }
        public int Level { get; set; } = 0;
        public string Type { get; set; } = "general";
        public bool IsActive { get; set; } = true;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Category> Children { get; set; } = new List<Category>();
    }
}
