using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using EInsurance.Security;
using Microsoft.AspNetCore.Identity;

namespace EInsurance.Services.Authentication;

public class UserAuthenticationService(
    IAuthenticationRepository authenticationRepository,
    PasswordHasher<object> passwordHasher) : IUserAuthenticationService
{
    public async Task<AuthenticatedUser?> AuthenticateAsync(string identifier, string password, CancellationToken cancellationToken = default)
    {
        var normalizedIdentifier = identifier.Trim();

        var admin = await authenticationRepository.GetAdminByIdentifierAsync(normalizedIdentifier, cancellationToken);
        if (admin is not null && VerifyPassword(admin.PasswordHash, password))
        {
            return new AuthenticatedUser(admin.AdminId, admin.FullName, admin.Email, RoleNames.Admin);
        }

        var employee = await authenticationRepository.GetEmployeeByIdentifierAsync(normalizedIdentifier, cancellationToken);
        if (employee is not null && VerifyPassword(employee.PasswordHash, password))
        {
            return new AuthenticatedUser(employee.EmployeeId, employee.FullName, employee.Email, RoleNames.Employee);
        }

        var insuranceAgent = await authenticationRepository.GetInsuranceAgentByIdentifierAsync(normalizedIdentifier, cancellationToken);
        if (insuranceAgent is not null && VerifyPassword(insuranceAgent.PasswordHash, password))
        {
            return new AuthenticatedUser(insuranceAgent.AgentId, insuranceAgent.FullName, insuranceAgent.Email, RoleNames.InsuranceAgent);
        }

        var customer = await authenticationRepository.GetCustomerByIdentifierAsync(normalizedIdentifier, cancellationToken);
        if (customer is not null && VerifyPassword(customer.PasswordHash, password))
        {
            return new AuthenticatedUser(customer.CustomerId, customer.FullName, customer.Email, RoleNames.Customer);
        }

        return null;
    }

    public async Task<AuthenticatedUser?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var admin = await authenticationRepository.GetAdminByIdAsync(userId, cancellationToken);
        if (admin is not null)
        {
            return new AuthenticatedUser(admin.AdminId, admin.FullName, admin.Email, RoleNames.Admin);
        }
        var employee = await authenticationRepository.GetEmployeeByIdAsync(userId, cancellationToken);
        if (employee is not null)
        {
            return new AuthenticatedUser(employee.EmployeeId, employee.FullName, employee.Email, RoleNames.Employee);
        }

        var insuranceAgent = await authenticationRepository.GetInsuranceAgentByIdAsync(userId, cancellationToken);
        if (insuranceAgent is not null)
        {
            return new AuthenticatedUser(insuranceAgent.AgentId, insuranceAgent.FullName, insuranceAgent.Email, RoleNames.InsuranceAgent);
        }

        var customer = await authenticationRepository.GetCustomerByIdAsync(userId, cancellationToken);
        if (customer is not null)
        {
            return new AuthenticatedUser(customer.CustomerId, customer.FullName, customer.Email, RoleNames.Customer);
        }

        return null;
    }

    private bool VerifyPassword(string passwordHash, string password)
    {
        var verificationResult = passwordHasher.VerifyHashedPassword(new object(), passwordHash, password);
        return verificationResult is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
