using EInsurance.Interfaces;
using EInsurance.Models.Policies;

namespace EInsurance.Services.Policies;

public class PolicyService(IPolicyRepository policyRepository) : IPolicyService
{
    public async Task<CustomerPoliciesViewModel?> GetCustomerPoliciesAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var customerPolicies = await policyRepository.GetCustomerPoliciesAsync(customerId, cancellationToken);

        if (customerPolicies is null)
        {
            return null;
        }

        return new CustomerPoliciesViewModel
        {
            CustomerId = customerPolicies.CustomerId,
            CustomerName = customerPolicies.FullName,
            CustomerEmail = customerPolicies.Email,
            Policies = customerPolicies.Policies.Select(policy => new PolicyDetailsViewModel
            {
                PolicyId = policy.PolicyId,
                PlanName = policy.PlanName,
                SchemeName = policy.SchemeName,
                PolicyDetails = policy.PolicyDetails,
                Premium = policy.Premium,
                DateIssued = policy.DateIssued,
                MaturityPeriod = policy.MaturityPeriod,
                PolicyLapseDate = policy.PolicyLapseDate,
                Payments = policy.Payments.Select(payment => new PaymentDetailsViewModel
                {
                    PaymentId = payment.PaymentId,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate
                }).ToList()
            }).ToList()
        };
    }

    public async Task<List<CustomerLookupViewModel>> SearchCustomersAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return [];
        }

        var customers = await policyRepository.SearchCustomersAsync(searchTerm, cancellationToken);

        return customers.Select(customer => new CustomerLookupViewModel
        {
            CustomerId = customer.CustomerId,
            FullName = customer.FullName,
            Email = customer.Email
        }).ToList();
    }
}
