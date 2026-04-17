using System.ComponentModel.DataAnnotations.Schema;

namespace EInsurance.Domain.Common;

public abstract class AuditableEntity
{
    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
