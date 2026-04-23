using EInsurance.Data;
using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using EInsurance.Services.Policies;
using Microsoft.EntityFrameworkCore;

namespace EInsurance.Repository;

public class PolicyRepository(ApplicationDbContext dbContext) : IPolicyRepository
{
    public async Task<List<CustomerLookupDto>> SearchCustomersAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var normalizedSearchTerm = searchTerm.Trim();

        return await dbContext.Customers.AsNoTracking()
            .Where(customer => customer.FullName.Contains(normalizedSearchTerm) || customer.Email.Contains(normalizedSearchTerm))
            .OrderBy(customer => customer.FullName)
            .Select(customer => new CustomerLookupDto(
                customer.CustomerId,
                customer.FullName,
                customer.Email))
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerPoliciesDto?> GetCustomerPoliciesAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers.AsNoTracking()
            .Where(customer => customer.CustomerId == customerId)
            .Select(customer => new CustomerPoliciesDto(
                customer.CustomerId,
                customer.FullName,
                customer.Email,
                customer.DateOfBirth,
                customer.Policies
                    .OrderByDescending(policy => policy.DateIssued)
                    .Select(policy => new PolicyDetailsDto(
                        policy.PolicyId,
                        policy.Scheme.Plan.PlanName,
                        policy.Scheme.SchemeName,
                        policy.PolicyDetails,
                        policy.Premium,
                        policy.DateIssued,
                        policy.MaturityPeriod,
                        policy.PolicyLapseDate,
                        policy.Payments
                            .OrderByDescending(payment => payment.PaymentDate)
                            .Select(payment => new PaymentDetailsDto(
                                payment.PaymentId,
                                payment.Amount,
                                payment.PaymentDate))
                            .ToList()))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Scheme>> GetAvailableSchemesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Schemes
            .Include(s => s.Plan)
            .AsNoTracking()
            .OrderBy(s => s.Plan.PlanName)
            .ThenBy(s => s.SchemeName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Scheme?> GetSchemeByIdAsync(int schemeId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Schemes
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.SchemeId == schemeId, cancellationToken);
    }

    public async Task<Policy> CreatePolicyAsync(Policy policy, CancellationToken cancellationToken = default)
    {
        dbContext.Policies.Add(policy);
        await dbContext.SaveChangesAsync(cancellationToken);
        return policy;
    }

    public async Task<Payment> CreatePaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        dbContext.Payments.Add(payment);
        await dbContext.SaveChangesAsync(cancellationToken);
        return payment;
    }

    public async Task UpdatePaymentStatusAsync(string paymentIntentId, string status, string? failureReason, CancellationToken cancellationToken = default)
    {
        var payment = await dbContext.Payments
            .FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntentId, cancellationToken);

        if (payment != null)
        {
            payment.PaymentStatus = status;
            payment.FailureReason = failureReason;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
