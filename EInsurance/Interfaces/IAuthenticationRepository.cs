using EInsurance.Domain.Entities;

namespace EInsurance.Interfaces;

public interface IAuthenticationRepository
{
    Task<Admin?> GetAdminByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    Task<Employee?> GetEmployeeByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    Task<InsuranceAgent?> GetInsuranceAgentByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    Task<Customer?> GetCustomerByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);

    Task<Admin?> GetAdminByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<InsuranceAgent?> GetInsuranceAgentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Customer?> GetCustomerByIdAsync(int id, CancellationToken cancellationToken = default);
}
