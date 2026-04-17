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

        await developmentSeedRepository.SaveChangesAsync(cancellationToken);
    }

    private string HashPassword(string password) => passwordHasher.HashPassword(new object(), password);
}
