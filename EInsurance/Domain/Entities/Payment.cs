using EInsurance.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EInsurance.Domain.Entities;

[Table("Payment")]
public class Payment : AuditableEntity
{
    [Key]
    [Column("PaymentID")]
    public int PaymentId { get; set; }

    [Column("CustomerID")]
    public int CustomerId { get; set; }

    [Column("PolicyID")]
    public int PolicyId { get; set; }

    [Precision(18, 2)]
    public decimal Amount { get; set; }

    [Column(TypeName = "date")]
    public DateOnly PaymentDate { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    [ForeignKey(nameof(PolicyId))]
    public Policy Policy { get; set; } = null!;
}
