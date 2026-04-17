using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.API.Models
{
    public class NotificationAnalytic
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? CampaignId { get; set; }
        [ForeignKey("CampaignId")]
        public NotificationCampaign? Campaign { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public int TotalSent { get; set; } = 0;
        public int TotalDelivered { get; set; } = 0;
        public int TotalOpened { get; set; } = 0;
        public int TotalClicked { get; set; } = 0;
        public int TotalFailed { get; set; } = 0;
        [Column(TypeName = "decimal(5,2)")]
        public decimal? DeliveryRate { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal? OpenRate { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal? ClickRate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
