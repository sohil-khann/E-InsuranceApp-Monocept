namespace EInsurance.Models.Policies;

public class PolicyDetailsViewModel
{
    public int PolicyId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string SchemeName { get; set; } = string.Empty;
    public string PolicyDetails { get; set; } = string.Empty;
    public decimal Premium { get; set; }
    public DateOnly DateIssued { get; set; }
    public int MaturityPeriod { get; set; }
    public DateOnly PolicyLapseDate { get; set; }
    public List<PaymentDetailsViewModel> Payments { get; set; } = [];
}
