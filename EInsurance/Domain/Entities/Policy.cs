using EInsurance.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EInsurance.Domain.Entities;

[Table("Policy")]
public class Policy : AuditableEntity
{
    [Key]
    [Column("PolicyID")]
    public int PolicyId { get; set; }

    [Column("CustomerID")]
    public int CustomerId { get; set; }

    [Column("SchemeID")]
    public int SchemeId { get; set; }

    [Required]
    public string PolicyDetails { get; set; } = string.Empty;

    [Precision(18, 2)]
    public decimal Premium { get; set; }

    [Column(TypeName = "date")]
    public DateOnly DateIssued { get; set; }

    public int MaturityPeriod { get; set; }

    [Column(TypeName = "date")]
    public DateOnly PolicyLapseDate { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    [ForeignKey(nameof(SchemeId))]
    public Scheme Scheme { get; set; } = null!;

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Commission> Commissions { get; set; } = new List<Commission>();
}
