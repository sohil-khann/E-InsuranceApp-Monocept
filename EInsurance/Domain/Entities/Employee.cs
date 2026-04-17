using EInsurance.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EInsurance.Domain.Entities;

[Table("Employee")]
[Index(nameof(Username), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class Employee : AccountEntity
{
    [Key]
    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;

    public ICollection<EmployeeScheme> EmployeeSchemes { get; set; } = new List<EmployeeScheme>();
}
