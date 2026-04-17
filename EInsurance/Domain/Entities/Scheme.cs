using EInsurance.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EInsurance.Domain.Entities;

[Table("Scheme")]
public class Scheme : AuditableEntity
{
    [Key]
    [Column("SchemeID")]
    public int SchemeId { get; set; }

    [Required]
    [MaxLength(100)]
    public string SchemeName { get; set; } = string.Empty;

    [Required]
    public string SchemeDetails { get; set; } = string.Empty;

    [Column("PlanID")]
    public int PlanId { get; set; }

    [ForeignKey(nameof(PlanId))]
    public InsurancePlan Plan { get; set; } = null!;

    public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    public ICollection<EmployeeScheme> EmployeeSchemes { get; set; } = new List<EmployeeScheme>();
}
