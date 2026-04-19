using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class PurchaseRequisitionItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RequisitionId { get; set; }
        [ForeignKey("RequisitionId")]
        public PurchaseRequisition? Requisition { get; set; }
        public Guid MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public RawMaterial? Material { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal QuantityRequired { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal? UnitCost { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? TotalCost { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
