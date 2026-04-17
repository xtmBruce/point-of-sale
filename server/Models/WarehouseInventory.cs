using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class WarehouseInventory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WarehouseId { get; set; }
        [ForeignKey("WarehouseId")]
        public Warehouse? Warehouse { get; set; }
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        public int Quantity { get; set; } = 0;
        public int MinStockLevel { get; set; } = 0;
        public int? MaxStockLevel { get; set; }
        public int ReorderPoint { get; set; } = 0;
        public int SafetyStock { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
