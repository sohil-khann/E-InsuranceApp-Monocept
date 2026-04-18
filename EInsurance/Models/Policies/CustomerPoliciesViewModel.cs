namespace EInsurance.Models.Policies;

public class CustomerPoliciesViewModel
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public List<PolicyDetailsViewModel> Policies { get; set; } = [];
}
