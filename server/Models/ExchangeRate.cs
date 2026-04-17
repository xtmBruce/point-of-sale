using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class ExchangeRate
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string BaseCurrency { get; set; } = string.Empty;
        public string TargetCurrency { get; set; } = string.Empty;
        [Column(TypeName = "decimal(15,6)")]
        public decimal Rate { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
