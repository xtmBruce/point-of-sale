using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class Discount
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public string Type { get; set; } = string.Empty; // percentage, fixed_amount, bottle_return
        [Column(TypeName = "decimal(10,2)")]
        public decimal Value { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? MinPurchaseAmount { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? MaxDiscountAmount { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? UsageLimit { get; set; }
        public int? UsagePerCustomer { get; set; }
        public string ApplicableTo { get; set; } = "all";
        public string? CustomerTiers { get; set; }
        public int? BottleReturnCount { get; set; }
        public bool IsActive { get; set; } = true;
        public bool AutoApply { get; set; } = false;
        public string DiscountType { get; set; } = "regular_discount";
        public bool AllowPartialPayment { get; set; } = false;
        public Guid? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public User? Creator { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
