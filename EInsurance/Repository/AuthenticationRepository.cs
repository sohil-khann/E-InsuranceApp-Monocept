using EInsurance.Data;
using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EInsurance.Repository;

public class AuthenticationRepository(ApplicationDbContext dbContext) : IAuthenticationRepository
{
    public Task<Admin?> GetAdminByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return dbContext.Admins.AsNoTracking()
            .FirstOrDefaultAsync(user => user.Username == identifier || user.Email == identifier, cancellationToken);
    }

    public Task<Employee?> GetEmployeeByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return dbContext.Employees.AsNoTracking()
            .FirstOrDefaultAsync(user => user.Username == identifier || user.Email == identifier, cancellationToken);
    }

    public Task<InsuranceAgent?> GetInsuranceAgentByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return dbContext.InsuranceAgents.AsNoTracking()
            .FirstOrDefaultAsync(user => user.Username == identifier || user.Email == identifier, cancellationToken);
    }

    public Task<Customer?> GetCustomerByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return dbContext.Customers.AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == identifier, cancellationToken);
    }

    public Task<Admin?> GetAdminByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Admins.AsNoTracking()
            .FirstOrDefaultAsync(user => user.AdminId == id, cancellationToken);
    }

    public Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Employees.AsNoTracking()
            .FirstOrDefaultAsync(user => user.EmployeeId == id, cancellationToken);
    }

    public Task<InsuranceAgent?> GetInsuranceAgentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.InsuranceAgents.AsNoTracking()
            .FirstOrDefaultAsync(user => user.AgentId == id, cancellationToken);
    }

    public Task<Customer?> GetCustomerByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Customers.AsNoTracking()
            .FirstOrDefaultAsync(user => user.CustomerId == id, cancellationToken);
    }
}
