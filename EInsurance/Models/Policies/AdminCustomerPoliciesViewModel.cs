namespace EInsurance.Models.Policies;

public class AdminCustomerPoliciesViewModel
{
    public string SearchTerm { get; set; } = string.Empty;
    public List<CustomerLookupViewModel> SearchResults { get; set; } = [];
    public CustomerPoliciesViewModel? SelectedCustomerPolicies { get; set; }
}
