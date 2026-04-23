using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using EInsurance.Models.Policies;
using EInsurance.Security;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EInsurance.Services.Policies;

public class PolicyService(
    IPolicyRepository policyRepository,
    IDataValidationService validationService) : IPolicyService
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
            DateOfBirth = customerPolicies.DateOfBirth,
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
            SchemeDetails = GetSchemeDescription(s.SchemeDetails),
            PlanName = s.Plan.PlanName,
            BasePremium = GetSchemeBasePremium(s.SchemeDetails)
        }).ToList();
    }

    public async Task<PurchaseConfirmationViewModel?> PurchasePolicyAsync(int customerId, PurchasePolicyViewModel model, CancellationToken cancellationToken = default)
    {
        var scheme = await policyRepository.GetSchemeByIdAsync(model.SchemeId, cancellationToken);
        if (scheme is null) return null;

        var customerPolicies = await policyRepository.GetCustomerPoliciesAsync(customerId, cancellationToken);
        if (customerPolicies != null)
        {
            var ageValidation = await validationService.ValidateCustomerAgeAsync(customerPolicies.DateOfBirth, cancellationToken);
            if (!ageValidation.Success)
            {
                throw new InvalidOperationException(ageValidation.Errors.First().Message);
            }
        }

        var currencyValidation = validationService.ValidateCurrencyFormat(model.CoverageAmount);
        if (!currencyValidation.Success)
        {
            throw new InvalidOperationException(currencyValidation.Errors.First().Message);
        }

        var actualPremium = model.ExactPremiumAmount ?? (model.CoverageAmount * 0.05m);

        var policy = new Policy
        {
            CustomerId = customerId,
            SchemeId = model.SchemeId,
            PolicyDetails = $"Beneficiary: {model.BeneficiaryName}, Coverage: {model.CoverageAmount:C}, Maturity: {model.MaturityPeriod} months",
            Premium = actualPremium,
            DateIssued = DateOnly.FromDateTime(DateTime.UtcNow),
            MaturityPeriod = model.MaturityPeriod,
            PolicyLapseDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(model.MaturityPeriod))
        };

        var createdPolicy = await policyRepository.CreatePolicyAsync(policy, cancellationToken);

        var premiumAmount = createdPolicy.Premium;
        var paymentCurrencyValidation = validationService.ValidateCurrencyFormat(premiumAmount);
        if (!paymentCurrencyValidation.Success)
        {
            throw new InvalidOperationException(paymentCurrencyValidation.Errors.First().Message);
        }

        var payment = new Payment
        {
            CustomerId = customerId,
            PolicyId = createdPolicy.PolicyId,
            Amount = premiumAmount,
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

    private static string GetSchemeDescription(string schemeDetails)
    {
        if (string.IsNullOrWhiteSpace(schemeDetails))
        {
            return string.Empty;
        }

        try
        {
            using var doc = JsonDocument.Parse(schemeDetails);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty("description", out var descriptionElement) &&
                descriptionElement.ValueKind == JsonValueKind.String)
            {
                return descriptionElement.GetString() ?? string.Empty;
            }
        }
        catch
        {
            // Not JSON; treat as plain text.
        }

        return schemeDetails;
    }

    private static decimal GetSchemeBasePremium(string schemeDetails, decimal defaultBasePremium = 50.00m)
    {
        if (string.IsNullOrWhiteSpace(schemeDetails))
        {
            return defaultBasePremium;
        }

        try
        {
            using var doc = JsonDocument.Parse(schemeDetails);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty("basePremium", out var premiumElement) &&
                premiumElement.TryGetDecimal(out var basePremium))
            {
                return basePremium;
            }
        }
        catch
        {
            // Not JSON; fall back.
        }

        return defaultBasePremium;
    }
}
