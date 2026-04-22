using EInsurance.Data;
using EInsurance.Interfaces;
using EInsurance.Models.Auth;
using EInsurance.Security;
using EInsurance.Services.Authentication;
using EInsurance.Services.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EInsurance.Controllers;

[AllowAnonymous]
public class AccountController(
    IUserAuthenticationService authenticationService,
    IJwtTokenService jwtTokenService,
    IRegistrationService registrationService,
    ISessionService sessionService,
    ApplicationDbContext dbContext) : Controller
{
    private const string DeviceInfoKey = "DeviceInfo";

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await authenticationService.AuthenticateAsync(model.Identifier, model.Password, cancellationToken);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials.");
            return View(model);
        }

        var existingSessionId = await sessionService.GetActiveSessionIdAsync(user.UserId, cancellationToken);
        
        var deviceInfo = GetDeviceInfo();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        
        var sessionId = await sessionService.CreateSessionAsync(
            user.UserId, 
            user.Role, 
            deviceInfo, 
            ipAddress, 
            cancellationToken);

        var userWithSession = user with { SessionId = sessionId };
        var authenticationResult = jwtTokenService.GenerateToken(userWithSession, sessionId);

        Response.Cookies.Append(AuthConstants.TokenCookieName, authenticationResult.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = authenticationResult.ExpiresAtUtc
        });

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        var agents = dbContext.InsuranceAgents
            .OrderBy(a => a.FullName)
            .Select(a => new SelectListItem
            {
                Value = a.AgentId.ToString(),
                Text = $"{a.AgentId} : "+a.FullName 
            })
            .ToList();

        ViewBag.AgentList = agents;

        return View(new CustomerRegistrationViewModel
        {
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-18))
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(CustomerRegistrationViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AgentList = dbContext.InsuranceAgents
                .OrderBy(a => a.FullName)
                .Select(a => new SelectListItem
                {
                    Value = a.AgentId.ToString(),
                    Text = a.FullName
                })
                .ToList();
            return View(model);
        }

        var result = await registrationService.RegisterCustomerAsync(
            model.FullName,
            model.Email,
            model.Phone,
            model.DateOfBirth,
            model.Password,
            model.AgentId,
            cancellationToken);

        if (!result.Succeeded)
        {
            ViewBag.AgentList = dbContext.InsuranceAgents
                .OrderBy(a => a.FullName)
                .Select(a => new SelectListItem
                {
                    Value = a.AgentId.ToString(),
                    Text = a.FullName
                })
                .ToList();
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var sessionIdClaim = User.FindFirst("SessionId")?.Value;
        
        if (Guid.TryParse(sessionIdClaim, out var sessionId))
        {
            await sessionService.InvalidateSessionAsync(sessionId, cancellationToken);
        }
        
        Response.Cookies.Delete(AuthConstants.TokenCookieName);
        return RedirectToAction(nameof(Login));
    }

    private string GetDeviceInfo()
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        if (string.IsNullOrEmpty(userAgent))
        {
            return "Unknown Device";
        }
        
        if (userAgent.Length > 255)
        {
            userAgent = userAgent.Substring(0, 255);
        }
        
        return userAgent;
    }
}