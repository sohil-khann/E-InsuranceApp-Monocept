using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EInsurance.Domain.Entities;

[Table("UserSession")]
public class UserSession
{
    [Key]
    [Column("SessionID")]
    public Guid SessionId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string UserType { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string DeviceInfo { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "datetime2")]
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "datetime2")]
    public DateTime ExpiresAt { get; set; }

    public bool IsActive { get; set; } = true;
}