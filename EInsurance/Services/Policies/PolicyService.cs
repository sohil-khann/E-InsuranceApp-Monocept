using EInsurance.Domain.Entities;
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

    public async Task<List<SchemeListItemViewModel>> GetAvailableSchemesAsync(CancellationToken cancellationToken = default)
    {
        var schemes = await policyRepository.GetAvailableSchemesAsync(cancellationToken);

        return schemes.Select(s => new SchemeListItemViewModel
        {
            SchemeId = s.SchemeId,
            SchemeName = s.SchemeName,
            SchemeDetails = s.SchemeDetails,
            PlanName = s.Plan.PlanName
        }).ToList();
    }

    public async Task<PurchaseConfirmationViewModel?> PurchasePolicyAsync(int customerId, PurchasePolicyViewModel model, CancellationToken cancellationToken = default)
    {
        var scheme = await policyRepository.GetSchemeByIdAsync(model.SchemeId, cancellationToken);
        if (scheme is null) return null;

        var policy = new Policy
        {
            CustomerId = customerId,
            SchemeId = model.SchemeId,
            PolicyDetails = $"Beneficiary: {model.BeneficiaryName}, Coverage: {model.CoverageAmount:C}, Maturity: {model.MaturityPeriod} months",
            Premium = model.CoverageAmount * 0.05m, 
            DateIssued = DateOnly.FromDateTime(DateTime.UtcNow),
            MaturityPeriod = model.MaturityPeriod,
            PolicyLapseDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(model.MaturityPeriod))
        };

        var createdPolicy = await policyRepository.CreatePolicyAsync(policy, cancellationToken);

        var payment = new Payment
        {
            CustomerId = customerId,
            PolicyId = createdPolicy.PolicyId,
            Amount = createdPolicy.Premium,
            PaymentDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        await policyRepository.CreatePaymentAsync(payment, cancellationToken);

        return new PurchaseConfirmationViewModel
        {
            PolicyId = createdPolicy.PolicyId,
            PolicyNumber = $"POL-{createdPolicy.PolicyId:D6}",
            SchemeName = scheme.SchemeName,
            PremiumPaid = createdPolicy.Premium,
            ExpiryDate = createdPolicy.PolicyLapseDate
        };
    }
}
