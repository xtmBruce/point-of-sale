using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class CurrencyRate
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FromCurrency { get; set; } = string.Empty;
        public string ToCurrency { get; set; } = string.Empty;
        [Column(TypeName = "decimal(15,6)")]
        public decimal Rate { get; set; }
        public DateOnly RateDate { get; set; }
        public string? Source { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
