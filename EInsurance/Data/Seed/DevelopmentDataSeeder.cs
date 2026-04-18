using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EInsurance.Data.Seed;

public class DevelopmentDataSeeder(
    IDevelopmentSeedRepository developmentSeedRepository,
    PasswordHasher<object> passwordHasher)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await developmentSeedRepository.MigrateAsync(cancellationToken);

        if (!await developmentSeedRepository.HasAdminsAsync(cancellationToken))
        {
            await developmentSeedRepository.AddAdminAsync(new Admin
            {
                FullName = "System Administrator",
                Email = "admin@einsurance.local",
                Username = "admin",
                PasswordHash = HashPassword("Admin@123")
            }, cancellationToken);
        }

        if (!await developmentSeedRepository.HasEmployeesAsync(cancellationToken))
        {
            await developmentSeedRepository.AddEmployeeAsync(new Employee
            {
                FullName = "Operations Employee",
                Email = "employee@einsurance.local",
                Username = "employee",
                Role = "Operations",
                PasswordHash = HashPassword("Employee@123")
            }, cancellationToken);
        }

        if (!await developmentSeedRepository.HasInsuranceAgentsAsync(cancellationToken))
        {
            await developmentSeedRepository.AddInsuranceAgentAsync(new InsuranceAgent
            {
                FullName = "Field Agent",
                Email = "agent@einsurance.local",
                Username = "agent",
                PasswordHash = HashPassword("Agent@123")
            }, cancellationToken);
        }

        if (!await developmentSeedRepository.HasCustomersAsync(cancellationToken))
        {
            await developmentSeedRepository.AddCustomerAsync(new Customer
            {
                FullName = "Demo Customer",
                Email = "customer@einsurance.local",
                PasswordHash = HashPassword("Customer@123"),
                Phone = "9876543210",
                DateOfBirth = new DateOnly(1995, 6, 15)
            }, cancellationToken);
        }

        if (!await developmentSeedRepository.HasInsurancePlansAsync(cancellationToken))
        {
            await developmentSeedRepository.AddInsurancePlanAsync(new InsurancePlan
            {
                PlanName = "Life Secure Plan",
                PlanDetails = "Long-term protection plan for customers."
            }, cancellationToken);
        }

        await developmentSeedRepository.SaveChangesAsync(cancellationToken);

        if (!await developmentSeedRepository.HasSchemesAsync(cancellationToken))
        {
            var insurancePlan = await developmentSeedRepository.GetFirstInsurancePlanAsync(cancellationToken);

            if (insurancePlan is not null)
            {
                await developmentSeedRepository.AddSchemeAsync(new Scheme
                {
                    PlanId = insurancePlan.PlanId,
                    SchemeName = "Gold Protection Scheme",
                    SchemeDetails = "Balanced premium coverage with yearly payment tracking."
                }, cancellationToken);
            }
        }

        await developmentSeedRepository.SaveChangesAsync(cancellationToken);

        if (!await developmentSeedRepository.HasPoliciesAsync(cancellationToken))
        {
            var customer = await developmentSeedRepository.GetCustomerByEmailAsync("customer@einsurance.local", cancellationToken);
            var scheme = await developmentSeedRepository.GetFirstSchemeAsync(cancellationToken);

            if (customer is not null && scheme is not null)
            {
                var policy = new Policy
                {
                    CustomerId = customer.CustomerId,
                    SchemeId = scheme.SchemeId,
                    PolicyDetails = "Comprehensive life insurance coverage with flexible maturity options.",
                    Premium = 2500.00m,
                    DateIssued = new DateOnly(2026, 1, 15),
                    MaturityPeriod = 15,
                    PolicyLapseDate = new DateOnly(2041, 1, 15)
                };

                await developmentSeedRepository.AddPolicyAsync(policy, cancellationToken);
                await developmentSeedRepository.SaveChangesAsync(cancellationToken);

                await developmentSeedRepository.AddPaymentAsync(new Payment
                {
                    CustomerId = customer.CustomerId,
                    PolicyId = policy.PolicyId,
                    Amount = 2500.00m,
                    PaymentDate = new DateOnly(2026, 1, 15)
                }, cancellationToken);

                await developmentSeedRepository.AddPaymentAsync(new Payment
                {
                    CustomerId = customer.CustomerId,
                    PolicyId = policy.PolicyId,
                    Amount = 2500.00m,
                    PaymentDate = new DateOnly(2026, 2, 15)
                }, cancellationToken);
            }
        }

        await developmentSeedRepository.SaveChangesAsync(cancellationToken);
    }

    private string HashPassword(string password) => passwordHasher.HashPassword(new object(), password);
}
