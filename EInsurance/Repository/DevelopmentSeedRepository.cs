using EInsurance.Data;
using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EInsurance.Repository;

public class DevelopmentSeedRepository(ApplicationDbContext dbContext) : IDevelopmentSeedRepository
{
    public Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Database.MigrateAsync(cancellationToken);
    }

    public Task<bool> HasAdminsAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Admins.AnyAsync(cancellationToken);
    }

    public Task<bool> HasEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Employees.AnyAsync(cancellationToken);
    }

    public Task<bool> HasInsuranceAgentsAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.InsuranceAgents.AnyAsync(cancellationToken);
    }

    public Task<bool> HasCustomersAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Customers.AnyAsync(cancellationToken);
    }

    public Task<bool> HasInsurancePlansAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.InsurancePlans.AnyAsync(cancellationToken);
    }

    public Task<bool> HasSchemesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Schemes.AnyAsync(cancellationToken);
    }

    public Task<bool> HasPoliciesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Policies.AnyAsync(cancellationToken);
    }

    public Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return dbContext.Customers.FirstOrDefaultAsync(customer => customer.Email == email, cancellationToken);
    }

    public Task<InsurancePlan?> GetFirstInsurancePlanAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.InsurancePlans.OrderBy(plan => plan.PlanId).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Scheme?> GetFirstSchemeAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Schemes.OrderBy(scheme => scheme.SchemeId).FirstOrDefaultAsync(cancellationToken);
    }

    public Task AddAdminAsync(Admin admin, CancellationToken cancellationToken = default)
    {
        return dbContext.Admins.AddAsync(admin, cancellationToken).AsTask();
    }

    public Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        return dbContext.Employees.AddAsync(employee, cancellationToken).AsTask();
    }

    public Task AddInsuranceAgentAsync(InsuranceAgent insuranceAgent, CancellationToken cancellationToken = default)
    {
        return dbContext.InsuranceAgents.AddAsync(insuranceAgent, cancellationToken).AsTask();
    }

    public Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        return dbContext.Customers.AddAsync(customer, cancellationToken).AsTask();
    }

    public Task AddInsurancePlanAsync(InsurancePlan insurancePlan, CancellationToken cancellationToken = default)
    {
        return dbContext.InsurancePlans.AddAsync(insurancePlan, cancellationToken).AsTask();
    }

    public Task AddSchemeAsync(Scheme scheme, CancellationToken cancellationToken = default)
    {
        return dbContext.Schemes.AddAsync(scheme, cancellationToken).AsTask();
    }

    public Task AddPolicyAsync(Policy policy, CancellationToken cancellationToken = default)
    {
        return dbContext.Policies.AddAsync(policy, cancellationToken).AsTask();
    }

    public Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        return dbContext.Payments.AddAsync(payment, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
