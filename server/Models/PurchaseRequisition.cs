using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class PurchaseRequisition
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string RequisitionNumber { get; set; } = string.Empty;
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public Guid RequestedBy { get; set; }
        [ForeignKey("RequestedBy")]
        public User? RequestedByUser { get; set; }
        public string? Department { get; set; }
        public string Priority { get; set; } = "normal"; // "low", "normal", "high", "urgent"
        public string Status { get; set; } = "pending"; // "pending", "approved", "rejected", "cancelled"
        [Column(TypeName = "decimal(15,2)")]
        public decimal? TotalEstimatedCost { get; set; }
        public Guid? ApprovedBy { get; set; }
        [ForeignKey("ApprovedBy")]
        public User? ApprovedByUser { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PurchaseRequisitionItem> Items { get; set; } = new List<PurchaseRequisitionItem>();
    }
}
