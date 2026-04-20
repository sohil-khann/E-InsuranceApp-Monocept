using EInsurance.Interfaces;
using EInsurance.Models.Policies;
using EInsurance.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EInsurance.Controllers;

[Authorize(Roles = RoleNames.Customer)]
public class PolicyPurchaseController(
    IPolicyService policyService,
    IPremiumCalculationService premiumCalculationService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> AvailableSchemes()
    {
        var schemes = await policyService.GetAvailableSchemesAsync();
        return View(new AvailableSchemesViewModel { Schemes = schemes });
    }

    /// <summary>
    /// UC-5 Step 1-2: User selects a plan/scheme and system fetches details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> CalculatePremium(int schemeId)
    {
        var schemes = await policyService.GetAvailableSchemesAsync();
        var selectedScheme = schemes.FirstOrDefault(s => s.SchemeId == schemeId);

        if (selectedScheme == null)
            return NotFound();

        // Get customer's date of birth to calculate age
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var customerId))
            return Challenge();

        var customerPolicies = await policyService.GetCustomerPoliciesAsync(customerId);
        var dateOfBirth = customerPolicies?.DateOfBirth;

        var model = new PremiumCalculationInputViewModel
        {
            SchemeId = schemeId,
            SchemeName = selectedScheme.SchemeName,
            CustomerAge = dateOfBirth?.CalculateAge()
        };

        return View(model);
    }

    /// <summary>
    /// UC-5 Step 4: Calculate premium based on user inputs
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CalculatePremium(PremiumCalculationInputViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            if (!model.CustomerAge.HasValue)
                return BadRequest("Customer age is required for premium calculation.");

            var result = await premiumCalculationService.CalculatePremiumAsync(
                model.SchemeId,
                model.SumAssured,
                model.CustomerAge.Value,
                model.MaturityPeriodMonths);

            // Store calculation result in session/TempData for the next step
            TempData["PremiumCalculation"] = System.Text.Json.JsonSerializer.Serialize(result);
            TempData["BeneficiaryName"] = model.BeneficiaryName;

            return RedirectToAction(nameof(ConfirmPremium), new { schemeId = model.SchemeId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    /// <summary>
    /// UC-5 Step 5: Display calculated premium for confirmation
    /// </summary>
    [HttpGet]
    public IActionResult ConfirmPremium(int schemeId)
    {
        var calculationJson = TempData["PremiumCalculation"] as string;
        if (string.IsNullOrEmpty(calculationJson))
            return RedirectToAction(nameof(AvailableSchemes));

        var result = System.Text.Json.JsonSerializer.Deserialize<PremiumCalculationResultViewModel>(calculationJson);
        if (result == null)
            return RedirectToAction(nameof(AvailableSchemes));

        var confirmModel = new PremiumConfirmationViewModel
        {
            SchemeId = result.SchemeId,
            SchemeName = result.SchemeName,
            CalculatedPremium = result.CalculatedPremium,
            MaturityPeriodMonths = result.MaturityPeriodMonths,
            SumAssured = result.SumAssured,
            QuoteId = result.QuoteId,
            BeneficiaryName = (TempData["BeneficiaryName"] as string) ?? string.Empty
        };

        return View(confirmModel);
    }

    /// <summary>
    /// UC-5 Postconditions: Create policy with calculated premium
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPremium(PremiumConfirmationViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var customerId))
            return Challenge();

        // Convert to purchase model (for compatibility with existing service)
        var purchaseModel = new PurchasePolicyViewModel
        {
            SchemeId = model.SchemeId,
            SchemeName = model.SchemeName,
            MaturityPeriod = model.MaturityPeriodMonths,
            CoverageAmount = model.SumAssured,
            BeneficiaryName = model.BeneficiaryName,
            PaymentMethod = model.PaymentMethod
        };

        var confirmation = await policyService.PurchasePolicyAsync(customerId, purchaseModel);

        if (confirmation == null)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while processing your purchase.");
            return View(model);
        }

        TempData["SuccessMessage"] = $"Policy purchased successfully! Premium Amount: {model.CalculatedPremium:C}";
        return RedirectToAction(nameof(Confirmation), new { policyId = confirmation.PolicyId });
    }

    /// <summary>
    /// Legacy purchase flow (kept for backwards compatibility)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Purchase(int schemeId)
    {
        var schemes = await policyService.GetAvailableSchemesAsync();
        var selectedScheme = schemes.FirstOrDefault(s => s.SchemeId == schemeId);

        if (selectedScheme == null)
            return NotFound();

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
        if (!ModelState.IsValid)
            return View(model);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var customerId))
            return Challenge();

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
