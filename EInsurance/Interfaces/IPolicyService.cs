using EInsurance.Models.Policies;

namespace EInsurance.Interfaces;

public interface IPolicyService
{
    Task<CustomerPoliciesViewModel?> GetCustomerPoliciesAsync(int customerId, CancellationToken cancellationToken = default);
    Task<List<CustomerLookupViewModel>> SearchCustomersAsync(string searchTerm, CancellationToken cancellationToken = default);
}
