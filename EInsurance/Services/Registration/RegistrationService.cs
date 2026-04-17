using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EInsurance.Services.Registration;

public class RegistrationService(
    IRegistrationRepository registrationRepository,
    PasswordHasher<object> passwordHasher) : IRegistrationService
{
    public async Task<RegistrationResult> RegisterCustomerAsync(
        string fullName,
        string email,
        string phone,
        DateOnly dateOfBirth,
        string password,
        int? agentId = null,
        CancellationToken cancellationToken = default)
    {
        email = email.Trim();

        var customer = new Customer
        {
            FullName = fullName.Trim(),
            Email = email,
            Phone = phone.Trim(),
            DateOfBirth = dateOfBirth,
            PasswordHash = HashPassword(password),
            AgentId = agentId
        };

        try
        {
            await registrationRepository.AddCustomerAsync(customer, cancellationToken);
            await registrationRepository.SaveChangesAsync(cancellationToken);
            return RegistrationResult.Success("Customer account created successfully. You can now log in.");
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("Email") ?? false)
        {
            return RegistrationResult.Failure("Email is already in use.");
        }
        catch (DbUpdateException ex)
        {
            return RegistrationResult.Failure($"Registration failed: {ex.Message}");
        }
    }

    public async Task<RegistrationResult> RegisterInsuranceAgentAsync(
        string fullName,
        string email,
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        email = email.Trim();
        username = username.Trim();

        var insuranceAgent = new InsuranceAgent
        {
            FullName = fullName.Trim(),
            Email = email,
            Username = username,
            PasswordHash = HashPassword(password)
        };

        try
        {
            await registrationRepository.AddInsuranceAgentAsync(insuranceAgent, cancellationToken);
            await registrationRepository.SaveChangesAsync(cancellationToken);
            return RegistrationResult.Success("Insurance agent registered successfully.");
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("Email") ?? false)
        {
            return RegistrationResult.Failure("Email is already in use.");
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("Username") ?? false)
        {
            return RegistrationResult.Failure("Username is already in use.");
        }
        catch (DbUpdateException ex)
        {
            return RegistrationResult.Failure($"Registration failed: {ex.Message}");
        }
    }

    public async Task<RegistrationResult> RegisterEmployeeAsync(
        string fullName,
        string email,
        string username,
        string role,
        string password,
        CancellationToken cancellationToken = default)
    {
        email = email.Trim();
        username = username.Trim();

        var employee = new Employee
        {
            FullName = fullName.Trim(),
            Email = email,
            Username = username,
            Role = role.Trim(),
            PasswordHash = HashPassword(password)
        };

        try
        {
            await registrationRepository.AddEmployeeAsync(employee, cancellationToken);
            await registrationRepository.SaveChangesAsync(cancellationToken);
            return RegistrationResult.Success("Employee registered successfully.");
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("Email") ?? false)
        {
            return RegistrationResult.Failure("Email is already in use.");
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("Username") ?? false)
        {
            return RegistrationResult.Failure("Username is already in use.");
        }
        catch (DbUpdateException ex)
        {
            return RegistrationResult.Failure($"Registration failed: {ex.Message}");
        }
    }

    private string HashPassword(string password) => passwordHasher.HashPassword(new object(), password);
}
