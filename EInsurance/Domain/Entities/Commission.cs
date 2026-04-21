using EInsurance.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EInsurance.Domain.Entities;

[Table("Commission")]
public class Commission : AuditableEntity
{
    [Key]
    [Column("CommissionID")]
    public int CommissionId { get; set; }

    [Column("AgentID")]
    public int AgentId { get; set; }

    [Column("PolicyID")]
    public int PolicyId { get; set; }

    [Precision(18, 2)]
    public decimal CommissionAmount { get; set; }

    [MaxLength(50)]
    [Required]
    public string Status { get; set; } = "Pending";

 
    [Column(TypeName = "datetime2")]
    public DateTime? PaidAtUtc { get; set; }

    [ForeignKey(nameof(AgentId))]
    public InsuranceAgent Agent { get; set; } = null!;

    [ForeignKey(nameof(PolicyId))]
    public Policy Policy { get; set; } = null!;
}
