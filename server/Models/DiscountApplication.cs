using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class DiscountApplication
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        public Guid? DiscountId { get; set; }
        [ForeignKey("DiscountId")]
        public Discount? Discount { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal AmountApplied { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal? PercentageApplied { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal OriginalAmount { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalAmount { get; set; }
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }
}
