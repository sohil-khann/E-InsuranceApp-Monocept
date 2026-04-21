using EInsurance.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EInsurance.Domain.Entities;

[Table("InsuranceAgent")]
[Index(nameof(Username), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class InsuranceAgent : AccountEntity
{
    [Key]
    [Column("AgentID")]
    public int AgentId { get; set; }

  
    [Precision(5, 4)]
    public decimal CommissionRate { get; set; } = 0.10m;

    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public ICollection<Commission> Commissions { get; set; } = new List<Commission>();
}
