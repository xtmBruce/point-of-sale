using System;
using System.ComponentModel.DataAnnotations;

namespace SmartPOS.API.Models
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string? CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public bool IsActive => Revoked == null && DateTime.UtcNow <= Expires;
    }
}
