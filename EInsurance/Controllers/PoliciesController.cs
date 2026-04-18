using EInsurance.Interfaces;
using EInsurance.Models.Policies;
using EInsurance.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EInsurance.Controllers;

[Authorize]
public class PoliciesController(IPolicyService policyService) : Controller
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
