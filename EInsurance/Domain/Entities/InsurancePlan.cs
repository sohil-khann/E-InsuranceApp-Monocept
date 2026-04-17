using EInsurance.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EInsurance.Domain.Entities;

[Table("InsurancePlan")]
public class InsurancePlan : AuditableEntity
{
    [Key]
    [Column("PlanID")]
    public int PlanId { get; set; }

    [Required]
    [MaxLength(100)]
    public string PlanName { get; set; } = string.Empty;

    [Required]
    public string PlanDetails { get; set; } = string.Empty;

    public ICollection<Scheme> Schemes { get; set; } = new List<Scheme>();
}
