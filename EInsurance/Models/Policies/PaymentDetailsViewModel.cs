namespace EInsurance.Models.Policies;

public class PaymentDetailsViewModel
{
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly PaymentDate { get; set; }
}
