using System.ComponentModel.DataAnnotations;
using EInsurance.Security;

namespace EInsurance.Models.Payment;

public class PaymentViewModel
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int PolicyId { get; set; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Amount must have exactly 2 decimal places.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Payment date is required.")]
    [DataType(DataType.Date)]
    public DateOnly PaymentDate { get; set; }
}

public class PaymentCreateModel
{
    public int CustomerId { get; set; }
    public int PolicyId { get; set; }
    public string Amount { get; set; } = string.Empty;
    public DateOnly PaymentDate { get; set; }
}