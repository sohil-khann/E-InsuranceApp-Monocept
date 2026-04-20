using EInsurance.Interfaces;
using EInsurance.Models.Policies;
using EInsurance.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EInsurance.Controllers;

[Authorize(Roles = RoleNames.Customer)]
public class PolicyPurchaseController(IPolicyService policyService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> AvailableSchemes()
    {
        var schemes = await policyService.GetAvailableSchemesAsync();
        return View(new AvailableSchemesViewModel { Schemes = schemes });
    }

    [HttpGet]
    public async Task<IActionResult> Purchase(int schemeId)
    {
        var schemes = await policyService.GetAvailableSchemesAsync();
        var selectedScheme = schemes.FirstOrDefault(s => s.SchemeId == schemeId);
        
        if (selectedScheme == null) return NotFound();

        return View(new PurchasePolicyViewModel 
        { 
            SchemeId = schemeId, 
            SchemeName = selectedScheme.SchemeName 
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Purchase(PurchasePolicyViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var customerId))
        {
            return Challenge();
        }

        var confirmation = await policyService.PurchasePolicyAsync(customerId, model);

        if (confirmation == null)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while processing your purchase.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Policy purchased successfully!";
        return RedirectToAction(nameof(Confirmation), new { policyId = confirmation.PolicyId });
    }

    [HttpGet]
    public async Task<IActionResult> Confirmation(int policyId)
    {
        return View(policyId);
    }
}
