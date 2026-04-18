using EInsurance.Domain.Entities;

namespace EInsurance.Interfaces;

public interface IDevelopmentSeedRepository
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
    Task<bool> HasAdminsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasEmployeesAsync(CancellationToken cancellationToken = default);
    Task<bool> HasInsuranceAgentsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasCustomersAsync(CancellationToken cancellationToken = default);
    Task<bool> HasInsurancePlansAsync(CancellationToken cancellationToken = default);
    Task<bool> HasSchemesAsync(CancellationToken cancellationToken = default);
    Task<bool> HasPoliciesAsync(CancellationToken cancellationToken = default);
    Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<InsurancePlan?> GetFirstInsurancePlanAsync(CancellationToken cancellationToken = default);
    Task<Scheme?> GetFirstSchemeAsync(CancellationToken cancellationToken = default);
    Task AddAdminAsync(Admin admin, CancellationToken cancellationToken = default);
    Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default);
    Task AddInsuranceAgentAsync(InsuranceAgent insuranceAgent, CancellationToken cancellationToken = default);
    Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken = default);
    Task AddInsurancePlanAsync(InsurancePlan insurancePlan, CancellationToken cancellationToken = default);
    Task AddSchemeAsync(Scheme scheme, CancellationToken cancellationToken = default);
    Task AddPolicyAsync(Policy policy, CancellationToken cancellationToken = default);
    Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
