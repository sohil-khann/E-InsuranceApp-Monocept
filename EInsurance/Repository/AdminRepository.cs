using EInsurance.Data;
using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using EInsurance.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace EInsurance.Repository;

public class AdminRepository(ApplicationDbContext dbContext) : IAdminRepository
{
    public async Task<PagedResult<Policy>> GetPoliciesAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var skip = (query.PageNumber - 1) * query.PageSize;

        var queryable = dbContext.Policies
            .Include(p => p.Scheme)
                .ThenInclude(s => s.Plan)
            .Include(p => p.Customer)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();
            queryable = queryable.Where(p => 
                p.PolicyId.ToString().Contains(searchTerm) ||
                p.Customer.FullName.ToLower().Contains(searchTerm) ||
                p.Scheme.SchemeName.ToLower().Contains(searchTerm));
        }

        var totalRecords = await queryable.CountAsync(cancellationToken);

        queryable = query.SortBy?.ToLower() switch
        {
            "policyid" => query.SortDescending 
                ? queryable.OrderByDescending(p => p.PolicyId) 
                : queryable.OrderBy(p => p.PolicyId),
            "dateissued" => query.SortDescending 
                ? queryable.OrderByDescending(p => p.DateIssued) 
                : queryable.OrderBy(p => p.DateIssued),
            "premium" => query.SortDescending 
                ? queryable.OrderByDescending(p => p.Premium) 
                : queryable.OrderBy(p => p.Premium),
            "customername" => query.SortDescending 
                ? queryable.OrderByDescending(p => p.Customer.FullName) 
                : queryable.OrderBy(p => p.Customer.FullName),
            _ => queryable.OrderByDescending(p => p.DateIssued)
        };

        var items = await queryable
            .Skip(skip)
            .Take(query.PageSize)
            .Select(p => new Policy
            {
                PolicyId = p.PolicyId,
                CustomerId = p.CustomerId,
                SchemeId = p.SchemeId,
                PolicyDetails = p.PolicyDetails,
                Premium = p.Premium,
                DateIssued = p.DateIssued,
                MaturityPeriod = p.MaturityPeriod,
                PolicyLapseDate = p.PolicyLapseDate,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<Policy>(items, totalRecords, query.PageNumber, query.PageSize);
    }

    public async Task<PagedResult<Customer>> GetCustomersAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var skip = (query.PageNumber - 1) * query.PageSize;

        var queryable = dbContext.Customers
            .Include(c => c.Agent)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();
            queryable = queryable.Where(c => 
                c.FullName.ToLower().Contains(searchTerm) || 
                c.Email.ToLower().Contains(searchTerm) ||
                c.Phone.Contains(searchTerm));
        }

        var totalRecords = await queryable.CountAsync(cancellationToken);

        queryable = query.SortBy?.ToLower() switch
        {
            "customername" => query.SortDescending 
                ? queryable.OrderByDescending(c => c.FullName) 
                : queryable.OrderBy(c => c.FullName),
            "email" => query.SortDescending 
                ? queryable.OrderByDescending(c => c.Email) 
                : queryable.OrderBy(c => c.Email),
            "dateofbirth" => query.SortDescending 
                ? queryable.OrderByDescending(c => c.DateOfBirth) 
                : queryable.OrderBy(c => c.DateOfBirth),
            "createdat" => query.SortDescending 
                ? queryable.OrderByDescending(c => c.CreatedAt) 
                : queryable.OrderBy(c => c.CreatedAt),
            _ => queryable.OrderByDescending(c => c.CreatedAt)
        };

        var items = await queryable
            .Skip(skip)
            .Take(query.PageSize)
            .Select(c => new Customer
            {
                CustomerId = c.CustomerId,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                DateOfBirth = c.DateOfBirth,
                AgentId = c.AgentId,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<Customer>(items, totalRecords, query.PageNumber, query.PageSize);
    }

    public async Task<PagedResult<Payment>> GetPaymentsAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        var skip = (query.PageNumber - 1) * query.PageSize;

        var queryable = dbContext.Payments
            .Include(p => p.Policy)
                .ThenInclude(pol => pol.Scheme)
            .Include(p => p.Customer)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();
            queryable = queryable.Where(p => 
                p.PaymentId.ToString().Contains(searchTerm) ||
                p.Customer.FullName.ToLower().Contains(searchTerm) ||
                p.Policy.PolicyId.ToString().Contains(searchTerm));
        }

        var totalRecords = await queryable.CountAsync(cancellationToken);

        queryable = query.SortBy?.ToLower() switch
        {
            "paymentid" => query.SortDescending 
                ? queryable.OrderByDescending(p => p.PaymentId) 
                : queryable.OrderBy(p => p.PaymentId),
            "paymentdate" => query.SortDescending 
                ? queryable.OrderByDescending(p => p.PaymentDate) 
                : queryable.OrderBy(p => p.PaymentDate),
            "amount" => query.SortDescending 
                ? queryable.OrderByDescending(p => p.Amount) 
                : queryable.OrderBy(p => p.Amount),
            "customername" => query.SortDescending 
                ? queryable.OrderByDescending(p => p.Customer.FullName) 
                : queryable.OrderBy(p => p.Customer.FullName),
            _ => queryable.OrderByDescending(p => p.PaymentDate)
        };

        var items = await queryable
            .Skip(skip)
            .Take(query.PageSize)
            .Select(p => new Payment
            {
                PaymentId = p.PaymentId,
                CustomerId = p.CustomerId,
                PolicyId = p.PolicyId,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<Payment>(items, totalRecords, query.PageNumber, query.PageSize);
    }

    public async Task<PagedResult<Commission>> GetCommissionsAsync(PaginationQuery query, int? agentId = null, CancellationToken cancellationToken = default)
    {
        var skip = (query.PageNumber - 1) * query.PageSize;

        var queryable = dbContext.Commissions
            .Include(c => c.Agent)
            .AsNoTracking()
            .AsQueryable();

        if (agentId.HasValue)
        {
            queryable = queryable.Where(c => c.AgentId == agentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();
            queryable = queryable.Where(c => 
                c.Agent.FullName.ToLower().Contains(searchTerm) ||
                c.CommissionId.ToString().Contains(searchTerm));
        }

        var totalRecords = await queryable.CountAsync(cancellationToken);

        queryable = query.SortBy?.ToLower() switch
        {
            "commissionid" => query.SortDescending 
                ? queryable.OrderByDescending(c => c.CommissionId) 
                : queryable.OrderBy(c => c.CommissionId),
            "amount" => query.SortDescending 
                ? queryable.OrderByDescending(c => c.CommissionAmount) 
                : queryable.OrderBy(c => c.CommissionAmount),
            "paidat" => query.SortDescending 
                ? queryable.OrderByDescending(c => c.PaidAtUtc) 
                : queryable.OrderBy(c => c.PaidAtUtc),
            "agentname" => query.SortDescending 
                ? queryable.OrderByDescending(c => c.Agent.FullName) 
                : queryable.OrderBy(c => c.Agent.FullName),
            _ => queryable.OrderByDescending(c => c.CreatedAt)
        };

        var items = await queryable
            .Skip(skip)
            .Take(query.PageSize)
            .Select(c => new Commission
            {
                CommissionId = c.CommissionId,
                PolicyId = c.PolicyId,
                AgentId = c.AgentId,
                CommissionAmount = c.CommissionAmount,
                Status = c.Status,
                PaidAtUtc = c.PaidAtUtc,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<Commission>(items, totalRecords, query.PageNumber, query.PageSize);
    }
}