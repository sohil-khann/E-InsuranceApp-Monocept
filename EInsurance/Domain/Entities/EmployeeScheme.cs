using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EInsurance.Domain.Entities;

[Table("EmployeeScheme")]
[Index(nameof(EmployeeId), nameof(SchemeId), IsUnique = true)]
public class EmployeeScheme
{
    [Key]
    [Column("EmployeeSchemeID")]
    public int EmployeeSchemeId { get; set; }

    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [Column("SchemeID")]
    public int SchemeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    [ForeignKey(nameof(SchemeId))]
    public Scheme Scheme { get; set; } = null!;
}
