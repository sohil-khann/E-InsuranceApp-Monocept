using EInsurance.Domain.Entities;

namespace EInsurance.Interfaces;

public interface IDevelopmentSeedRepository
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
    Task<bool> HasAdminsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasEmployeesAsync(CancellationToken cancellationToken = default);
    Task<bool> HasInsuranceAgentsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasCustomersAsync(CancellationToken cancellationToken = default);
    Task AddAdminAsync(Admin admin, CancellationToken cancellationToken = default);
    Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default);
    Task AddInsuranceAgentAsync(InsuranceAgent insuranceAgent, CancellationToken cancellationToken = default);
    Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
