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
                Role = "Employee",
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

        await SeedPolicyCatalogAsync(cancellationToken);

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
                    MaturityPeriod = 180,
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

    private async Task SeedPolicyCatalogAsync(CancellationToken cancellationToken)
    {
        var plans = new[]
        {
            new
            {
                Name = "Life Secure Plan",
                Details = "Long-term protection plans for income continuity and family security.",
                Schemes = new[]
                {
                    new
                    {
                        Name = "Gold Protection Scheme",
                        Details = """{"description":"Balanced life coverage with steady benefits and yearly payment tracking.","basePremium":2500,"interestRate":0.05,"discount":0.02}"""
                    },
                    new
                    {
                        Name = "Family Shield Scheme",
                        Details = """{"description":"Family-focused life protection with higher coverage for dependents.","basePremium":3200,"interestRate":0.052,"surcharge":0.01}"""
                    },
                    new
                    {
                        Name = "Term Guard Scheme",
                        Details = """{"description":"Simple term coverage for essential protection at a leaner monthly cost.","basePremium":1400,"interestRate":0.045,"discount":0.04}"""
                    }
                }
            },
            new
            {
                Name = "Health Assurance Plan",
                Details = "Medical coverage options for hospitalisation and critical illness needs.",
                Schemes = new[]
                {
                    new
                    {
                        Name = "Essential Health Cover",
                        Details = """{"description":"Core medical coverage for hospitalisation, diagnostics, and routine care.","basePremium":1800,"interestRate":0.047,"discount":0.01}"""
                    },
                    new
                    {
                        Name = "Critical Care Plus",
                        Details = """{"description":"Expanded health cover for major illness, surgery, and recovery support.","basePremium":2900,"interestRate":0.055,"surcharge":0.02}"""
                    }
                }
            },
            new
            {
                Name = "Vehicle Protection Plan",
                Details = "Coverage options for private vehicles and commercial fleet protection.",
                Schemes = new[]
                {
                    new
                    {
                        Name = "Two-Wheeler Secure",
                        Details = """{"description":"Affordable protection for bikes and scooters with accident coverage.","basePremium":850,"interestRate":0.042,"discount":0.03}"""
                    },
                    new
                    {
                        Name = "Motor Comprehensive",
                        Details = """{"description":"Full vehicle coverage for damage, theft, liability, and roadside events.","basePremium":2100,"interestRate":0.05,"surcharge":0.01}"""
                    },
                    new
                    {
                        Name = "Fleet Comprehensive Coverage",
                        Details = """{"description":"Commercial vehicle protection for multi-vehicle operations and asset continuity.","basePremium":3420,"interestRate":0.056,"surcharge":0.025}"""
                    }
                }
            },
            new
            {
                Name = "Property Protection Plan",
                Details = "Protection for residential and commercial property risks.",
                Schemes = new[]
                {
                    new
                    {
                        Name = "Home Shield Cover",
                        Details = """{"description":"Residential property coverage for fire, theft, and natural calamity risk.","basePremium":1650,"interestRate":0.046,"discount":0.015}"""
                    },
                    new
                    {
                        Name = "Commercial Structural Liability",
                        Details = """{"description":"Commercial property and liability protection for structural risk exposure.","basePremium":1240,"interestRate":0.051,"surcharge":0.015}"""
                    }
                }
            }
        };

        foreach (var planSeed in plans)
        {
            var plan = await developmentSeedRepository.GetInsurancePlanByNameAsync(planSeed.Name, cancellationToken);

            if (plan is null)
            {
                plan = new InsurancePlan
                {
                    PlanName = planSeed.Name,
                    PlanDetails = planSeed.Details
                };

                await developmentSeedRepository.AddInsurancePlanAsync(plan, cancellationToken);
                await developmentSeedRepository.SaveChangesAsync(cancellationToken);
            }

            foreach (var schemeSeed in planSeed.Schemes)
            {
                var existingScheme = await developmentSeedRepository.GetSchemeByNameAsync(schemeSeed.Name, cancellationToken);

                if (existingScheme is not null)
                {
                    continue;
                }

                await developmentSeedRepository.AddSchemeAsync(new Scheme
                {
                    PlanId = plan.PlanId,
                    SchemeName = schemeSeed.Name,
                    SchemeDetails = schemeSeed.Details
                }, cancellationToken);
            }
        }
    }
}
