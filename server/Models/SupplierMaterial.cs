using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class SupplierMaterial
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }
        public Guid MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public RawMaterial? Material { get; set; }
        public string? MaterialCode { get; set; }
        public string? SupplierPartNumber { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal StandardCost { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal MinimumOrderQuantity { get; set; } = 1;
        public int LeadTimeDays { get; set; } = 0;
        public bool IsPreferred { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
