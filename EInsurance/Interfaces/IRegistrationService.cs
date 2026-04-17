using EInsurance.Services.Registration;

namespace EInsurance.Interfaces;

public interface IRegistrationService
{
    Task<RegistrationResult> RegisterCustomerAsync(
        string fullName,
        string email,
        string phone,
        DateOnly dateOfBirth,
        string password,
        int? agentId = null,
        CancellationToken cancellationToken = default);

    Task<RegistrationResult> RegisterInsuranceAgentAsync(
        string fullName,
        string email,
        string username,
        string password,
        CancellationToken cancellationToken = default);

    Task<RegistrationResult> RegisterEmployeeAsync(
        string fullName,
        string email,
        string username,
        string role,
        string password,
        CancellationToken cancellationToken = default);
}
