using EInsurance.Domain.Entities;
using EInsurance.Models.Common;

namespace EInsurance.Interfaces;

public interface IAdminRepository
{
    Task<PagedResult<Policy>> GetPoliciesAsync(PaginationQuery query, CancellationToken cancellationToken = default);
    Task<PagedResult<Customer>> GetCustomersAsync(PaginationQuery query, CancellationToken cancellationToken = default);
    Task<PagedResult<Payment>> GetPaymentsAsync(PaginationQuery query, CancellationToken cancellationToken = default);
    Task<PagedResult<Commission>> GetCommissionsAsync(PaginationQuery query, int? agentId = null, CancellationToken cancellationToken = default);
}