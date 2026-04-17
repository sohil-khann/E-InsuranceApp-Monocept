using System.ComponentModel.DataAnnotations;

namespace EInsurance.Domain.Common;

public abstract class AccountEntity : PersonEntity
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;
}
