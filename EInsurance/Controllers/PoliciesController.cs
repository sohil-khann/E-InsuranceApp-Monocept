using EInsurance.Interfaces;
using EInsurance.Models.Common;
using EInsurance.Models.Policies;
using EInsurance.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EInsurance.Controllers;

[Authorize]
public class PoliciesController(
    IPolicyService policyService,
    IAdminRepository adminRepository) : Controller
{
    [Authorize(Roles = RoleNames.Customer)]
    [HttpGet]
    public async Task<IActionResult> MyPolicies(CancellationToken cancellationToken)
    {
        var customerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(customerIdValue, out var customerId))
        {
            return Forbid();
        }

        var policies = await policyService.GetCustomerPoliciesAsync(customerId, cancellationToken);

        if (policies is null)
        {
            return NotFound();
        }

        return View(policies);
    }

    [Authorize(Roles = RoleNames.AdminOrEmployee)]
    [HttpGet]
    public async Task<IActionResult> AllPolicies(
        int pageNumber = 1,
        int pageSize = 20,
        string sortBy = "dateissued",
        bool sortDescending = true,
        string searchTerm = "",
        CancellationToken cancellationToken = default)
    {
        var query = new PaginationQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending,
            SearchTerm = searchTerm
        };

        var pagedPolicies = await adminRepository.GetPoliciesAsync(query, cancellationToken);

        var model = new AdminPoliciesViewModel
        {
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending,
            Policies = new PagedResult<PolicyListItemViewModel>(
                pagedPolicies.Items.Select(p => new PolicyListItemViewModel
                {
                    PolicyId = p.PolicyId,
                    Premium = p.Premium,
                    DateIssued = p.DateIssued,
                    MaturityPeriod = p.MaturityPeriod,
                    PolicyLapseDate = p.PolicyLapseDate,
                    CreatedAt = p.CreatedAt
                }).ToList(),
                pagedPolicies.TotalRecords,
                pagedPolicies.CurrentPage,
                pagedPolicies.PageSize)
        };

        return View(model);
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public async Task<IActionResult> CustomerPolicies(string searchTerm = "", int? customerId = null, CancellationToken cancellationToken = default)
    {
        var model = new AdminCustomerPoliciesViewModel
        {
            SearchTerm = searchTerm
        };

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            model.SearchResults = await policyService.SearchCustomersAsync(searchTerm, cancellationToken);
        }

        if (customerId.HasValue)
        {
            model.SelectedCustomerPolicies = await policyService.GetCustomerPoliciesAsync(customerId.Value, cancellationToken);
        }

        return View(model);
    }
}
