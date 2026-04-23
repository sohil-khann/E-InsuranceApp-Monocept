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
    IPremiumCalculationService premiumCalculationService,
    IConfiguration configuration) : Controller
{
    private string StripePublishableKey => configuration["Stripe:PublishableKey"] ?? string.Empty;
    [HttpGet]
    public async Task<IActionResult> AvailableSchemes()
    {
        var schemes = await policyService.GetAvailableSchemesAsync();
        return View(new AvailableSchemesViewModel { Schemes = schemes });
    }

    [HttpGet]
    public async Task<IActionResult> CalculatePremium(int schemeId)
    {
        var schemes = await policyService.GetAvailableSchemesAsync();
        var selectedScheme = schemes.FirstOrDefault(s => s.SchemeId == schemeId);

        if (selectedScheme == null)
            return NotFound();

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

  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPremium(PremiumConfirmationViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var customerId))
            return Challenge();

        TempData["PendingPolicySchemeId"] = model.SchemeId;
        TempData["PendingPolicySchemeName"] = model.SchemeName;
        TempData["PendingPolicyMaturity"] = model.MaturityPeriodMonths;
        TempData["PendingPolicyCoverage"] = model.SumAssured.ToString();
        TempData["PendingPolicyPremium"] = model.CalculatedPremium.ToString();
        TempData["PendingPolicyBeneficiary"] = model.BeneficiaryName;
        TempData["PendingPolicyQuoteId"] = model.QuoteId;

        return RedirectToAction(nameof(Payment), new { 
            quoteId = model.QuoteId,
            schemeId = model.SchemeId,
            schemeName = model.SchemeName,
            maturity = model.MaturityPeriodMonths,
            coverage = model.SumAssured.ToString(),
            premium = model.CalculatedPremium.ToString(),
            beneficiary = model.BeneficiaryName
        });
    }

    [HttpGet]
    public IActionResult Payment(string quoteId, string schemeId, string schemeName, string maturity, string coverage, string premium, string beneficiary)
    {
        if (!string.IsNullOrEmpty(premium))
        {
            TempData["PendingPolicySchemeId"] = schemeId;
            TempData["PendingPolicySchemeName"] = schemeName;
            TempData["PendingPolicyMaturity"] = maturity;
            TempData["PendingPolicyCoverage"] = coverage;
            TempData["PendingPolicyPremium"] = premium;
            TempData["PendingPolicyBeneficiary"] = beneficiary;
            TempData["PendingPolicyQuoteId"] = quoteId;
        }
        else
        {
            schemeId = TempData["PendingPolicySchemeId"] as string;
            schemeName = TempData["PendingPolicySchemeName"] as string;
            maturity = TempData["PendingPolicyMaturity"] as string;
            coverage = TempData["PendingPolicyCoverage"] as string;
            premium = TempData["PendingPolicyPremium"] as string;
            beneficiary = TempData["PendingPolicyBeneficiary"] as string;
        }

        if (string.IsNullOrEmpty(schemeId) || string.IsNullOrEmpty(premium))
        {
            return RedirectToAction(nameof(AvailableSchemes));
        }

        if (!decimal.TryParse(premium, out var premiumAmount))
        {
            return RedirectToAction(nameof(AvailableSchemes));
        }

        ViewData["StripePublishableKey"] = StripePublishableKey;
        ViewData["Amount"] = premiumAmount;
        ViewData["QuoteId"] = quoteId ?? TempData["PendingPolicyQuoteId"] as string;
        ViewData["SchemeId"] = schemeId;
        ViewData["SchemeName"] = schemeName;
        ViewData["MaturityPeriod"] = maturity;
        ViewData["CoverageAmount"] = coverage;
        ViewData["BeneficiaryName"] = beneficiary;

        return View();
    }
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
    public async Task<IActionResult> Confirmation(int policyId, bool paymentSuccess = false)
    {
        if (paymentSuccess)
        {
            TempData["SuccessMessage"] = "Payment successful! Your policy has been issued.";
        }
        return View(policyId);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessPaymentComplete([FromBody] PaymentCompleteRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var customerId))
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            if (!int.TryParse(TempData["PendingPolicySchemeId"] as string, out var schemeId) ||
                !int.TryParse(TempData["PendingPolicyMaturity"] as string, out var maturity) ||
                !decimal.TryParse(TempData["PendingPolicyCoverage"] as string, out var coverage) ||
                !decimal.TryParse(TempData["PendingPolicyPremium"] as string, out var premium))
            {
                return BadRequest(new { error = "Invalid payment data" });
            }

            var purchaseModel = new PurchasePolicyViewModel
            {
                SchemeId = schemeId,
                SchemeName = TempData["PendingPolicySchemeName"] as string ?? "",
                MaturityPeriod = maturity,
                CoverageAmount = coverage,
                BeneficiaryName = TempData["PendingPolicyBeneficiary"] as string ?? "",
                PaymentMethod = "Stripe",
                ExactPremiumAmount = premium
            };

            var confirmation = await policyService.PurchasePolicyAsync(customerId, purchaseModel);

            if (confirmation == null)
            {
                return BadRequest(new { error = "Failed to create policy" });
            }

            return Ok(new { policyId = confirmation.PolicyId, policyNumber = confirmation.PolicyNumber });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class PaymentCompleteRequest
{
    public string PaymentIntentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int PolicyId { get; set; }
}