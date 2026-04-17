using EInsurance.Interfaces;
using EInsurance.Models.Auth;
using EInsurance.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EInsurance.Controllers;

[Authorize]
public class RegistrationController(IRegistrationService registrationService) : Controller
{
    [Authorize(Roles = RoleNames.AdminOrEmployee)]
    [HttpGet]
    public IActionResult RegisterInsuranceAgent()
    {
        return View(new InsuranceAgentRegistrationViewModel());
    }

    [Authorize(Roles = RoleNames.AdminOrEmployee)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterInsuranceAgent(InsuranceAgentRegistrationViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await registrationService.RegisterInsuranceAgentAsync(
            model.FullName,
            model.Email,
            model.Username,
            model.Password,
            cancellationToken);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(RegisterInsuranceAgent));
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public IActionResult RegisterEmployee()
    {
        return View(new EmployeeRegistrationViewModel());
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterEmployee(EmployeeRegistrationViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await registrationService.RegisterEmployeeAsync(
            model.FullName,
            model.Email,
            model.Username,
            model.Role,
            model.Password,
            cancellationToken);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(RegisterEmployee));
    }
}
