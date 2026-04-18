namespace EInsurance.Interfaces;

public interface IPolicyRepository
{
    Task<List<CustomerLookupDto>> SearchCustomersAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<CustomerPoliciesDto?> GetCustomerPoliciesAsync(int customerId, CancellationToken cancellationToken = default);
}
