using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class PurchaseOrder
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string PONumber { get; set; } = string.Empty;
        [Required]
        public Guid SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string Status { get; set; } = "pending"; // "pending", "confirmed", "shipped", "received", "cancelled"
        [Column(TypeName = "decimal(15,2)")]
        [Required]
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "RWF";
        public string? PaymentTerms { get; set; }
        public string? ShippingAddress { get; set; }
        [Column(TypeName = "decimal(10,4)")]
        public decimal? CurrencyRate { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? TransportSupplierCost { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? BankCharges { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? TransportKigaliCost { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? LaisseSuivreCost { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? ImportTaxes { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? StorageCost { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? DeclarantFees { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? TransportWarehouseCost { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? TotalAmountRWF { get; set; }
        public string? Notes { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? CreatedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    }
}
