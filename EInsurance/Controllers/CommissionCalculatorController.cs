using EInsurance.Interfaces;
using EInsurance.Models.Commission;
using EInsurance.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EInsurance.Controllers;

[Authorize(Roles = RoleNames.Admin)]
[Route("Admin/Commission")]
public class CommissionCalculatorController(ICommissionCalculationService commissionService) : Controller
{
    
    [HttpGet("")]
    [HttpGet("Dashboard")]
    public async Task<IActionResult> Dashboard(int? agentId)
    {
        if (agentId.HasValue)
        {
            var dashboard = await commissionService.GetAgentDashboardAsync(agentId.Value);
            if (dashboard == null)
                return NotFound("Agent not found");

            return View("Dashboard", dashboard);
        }

        var agents = await commissionService.GetAllAgentsAsync();
        var selectModel = new CommissionCalculationInputViewModel
        {
            FromDate = DateTime.UtcNow.AddMonths(-1),
            ToDate = DateTime.UtcNow
        };

        ViewBag.Agents = agents;
        return View("SelectAgent", selectModel);
    }

   
    [HttpGet("Calculator")]
    public async Task<IActionResult> Calculator()
    {
        var agents = await commissionService.GetAllAgentsAsync();
        ViewBag.Agents = agents;

        var model = new CommissionCalculationInputViewModel
        {
            FromDate = DateTime.UtcNow.AddMonths(-1),
            ToDate = DateTime.UtcNow
        };

        return View(model);
    }

    
    [HttpPost("Calculate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Calculate(CommissionCalculationInputViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var agents = await commissionService.GetAllAgentsAsync();
            ViewBag.Agents = agents;
            return View("Calculator", model);
        }

        var result = await commissionService.CalculateAgentCommissionAsync(
            model.AgentId,
            model.FromDate,
            model.ToDate);

        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "Failed to calculate commission for the selected agent.");
            var agents = await commissionService.GetAllAgentsAsync();
            ViewBag.Agents = agents;
            return View("Calculator", model);
        }

        return View("CalculationResult", result);
    }

    [HttpPost("Finalize")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Finalize(int agentId, DateTime fromDate, DateTime toDate)
    {
        try
        {
            var report = await commissionService.FinalizeCommissionAsync(agentId, fromDate, toDate);

            if (report == null)
            {
                TempData["ErrorMessage"] = "No commission data to finalize.";
                return RedirectToAction(nameof(Calculator));
            }

            if (report.Status == "AlreadyFinalized")
            {
                TempData["WarningMessage"] = "Commission for these policies has already been finalized previously.";
            }
            else
            {
                TempData["SuccessMessage"] = $"Commission finalized successfully for {report.AgentName}!";
            }
            
            return View("CommissionReport", report);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error finalizing commission: {ex.Message}";
            return RedirectToAction(nameof(Calculator));
        }
    }

    
    [HttpPost("BatchCalculate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BatchCalculate(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var results = await commissionService.CalculateAllAgentCommissionsAsync(fromDate, toDate);

            if (results.Count == 0)
            {
                TempData["WarningMessage"] = "No commission data found for the specified period.";
                return RedirectToAction(nameof(Calculator));
            }

            ViewBag.TotalCommission = results.Sum(r => r.TotalCommissionAmount);
            ViewBag.TotalAgents = results.Count;
            ViewBag.TotalPolicies = results.Sum(r => r.TotalPoliciesCount);

            return View("BatchResults", results);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error in batch calculation: {ex.Message}";
            return RedirectToAction(nameof(Calculator));
        }
    }

    [HttpPost("MarkAsPaid/{commissionId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsPaid(int commissionId, int agentId)
    {
        var success = await commissionService.MarkCommissionAsPaidAsync(commissionId);

        if (!success)
        {
            TempData["ErrorMessage"] = "Failed to mark commission as paid.";
        }
        else
        {
            TempData["SuccessMessage"] = "Commission marked as paid successfully.";
        }

        return RedirectToAction(nameof(Dashboard), new { agentId });
    }

  
    [HttpGet("Ledger/{agentId}")]
    public async Task<IActionResult> Ledger(int agentId, int pageNumber = 1)
    {
        const int pageSize = 20;

        var ledger = await commissionService.GetCommissionLedgerAsync(agentId, pageNumber, pageSize);

        ViewBag.AgentId = agentId;
        ViewBag.CurrentPage = pageNumber;
        ViewBag.PageSize = pageSize;

        return View(ledger);
    }
}
