namespace EInsurance.Models.Policies;

public class CustomerLookupViewModel
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
