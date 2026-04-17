using EInsurance.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EInsurance.Controllers;

[Authorize]
public class DashboardController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var role = User.FindFirstValue(ClaimTypes.Role) ?? "User";
        var displayName = User.Identity?.Name ?? "User";

        var model = new DashboardViewModel
        {
            DisplayName = displayName,
            Role = role,
            Message = $"{role} access granted."
        };

        return View(model);
    }
}
