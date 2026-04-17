using EInsurance.Data;
using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EInsurance.Repository;

public class RegistrationRepository(ApplicationDbContext dbContext) : IRegistrationRepository
{
    public async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default)
    {
        return await dbContext.Admins.AnyAsync(user => user.Email == email, cancellationToken)
            || await dbContext.Employees.AnyAsync(user => user.Email == email, cancellationToken)
            || await dbContext.InsuranceAgents.AnyAsync(user => user.Email == email, cancellationToken)
            || await dbContext.Customers.AnyAsync(user => user.Email == email, cancellationToken);
    }

    public async Task<bool> IsUsernameTakenAsync(string username, CancellationToken cancellationToken = default)
    {
        return await dbContext.Admins.AnyAsync(user => user.Username == username, cancellationToken)
            || await dbContext.Employees.AnyAsync(user => user.Username == username, cancellationToken)
            || await dbContext.InsuranceAgents.AnyAsync(user => user.Username == username, cancellationToken);
    }

    public Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        return dbContext.Customers.AddAsync(customer, cancellationToken).AsTask();
    }

    public Task AddInsuranceAgentAsync(InsuranceAgent insuranceAgent, CancellationToken cancellationToken = default)
    {
        return dbContext.InsuranceAgents.AddAsync(insuranceAgent, cancellationToken).AsTask();
    }

    public Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        return dbContext.Employees.AddAsync(employee, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
