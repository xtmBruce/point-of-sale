using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class InventoryTransaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        public Guid? ShopId { get; set; }
        [ForeignKey("ShopId")]
        public Shop? Shop { get; set; }
        public Guid? WarehouseId { get; set; }
        [ForeignKey("WarehouseId")]
        public Warehouse? Warehouse { get; set; }
        [Required]
        public string TransactionType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int? PreviousStock { get; set; }
        public int? NewStock { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? UnitCost { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? TotalValue { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public Guid? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
        public string? Notes { get; set; }
        public Guid? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? CreatedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
