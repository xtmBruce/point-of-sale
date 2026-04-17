using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class CurrencyRate
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string FromCurrency { get; set; } = string.Empty;
        [Required]
        public string ToCurrency { get; set; } = string.Empty;
        [Column(TypeName = "decimal(15,6)")]
        [Required]
        public decimal Rate { get; set; }
        [Required]
        public DateTime RateDate { get; set; }
        public string? Source { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
