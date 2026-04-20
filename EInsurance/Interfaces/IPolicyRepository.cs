using EInsurance.Domain.Entities;
using EInsurance.Services.Policies;

namespace EInsurance.Interfaces;

public interface IPolicyRepository
{
    Task<List<CustomerLookupDto>> SearchCustomersAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<CustomerPoliciesDto?> GetCustomerPoliciesAsync(int customerId, CancellationToken cancellationToken = default);
    Task<List<Scheme>> GetAvailableSchemesAsync(CancellationToken cancellationToken = default);
    Task<Scheme?> GetSchemeByIdAsync(int schemeId, CancellationToken cancellationToken = default);
    Task<Policy> CreatePolicyAsync(Policy policy, CancellationToken cancellationToken = default);
    Task<Payment> CreatePaymentAsync(Payment payment, CancellationToken cancellationToken = default);
}
