using EInsurance.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EInsurance.Domain.Entities;

[Table("Customer")]
[Index(nameof(Email), IsUnique = true)]
public class Customer : PersonEntity
{
    [Key]
    [Column("CustomerID")]
    public int CustomerId { get; set; }

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    public string Phone { get; set; } = string.Empty;

    [Column(TypeName = "date")]
    public DateOnly DateOfBirth { get; set; }

    [Column("AgentID")]
    public int? AgentId { get; set; }

    [ForeignKey(nameof(AgentId))]
    public InsuranceAgent? Agent { get; set; }

    public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
