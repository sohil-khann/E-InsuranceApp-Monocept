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

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
