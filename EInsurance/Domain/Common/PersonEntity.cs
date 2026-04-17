using System.ComponentModel.DataAnnotations;

namespace EInsurance.Domain.Common;

public abstract class PersonEntity : AuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
}
