using EInsurance.Interfaces;
using EInsurance.Services.Session;
using System.Security.Claims;

namespace EInsurance.Middleware;

public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;

    public SessionValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        
        // Allow anonymous routes
        if (path == "/" || path == "/home" || path == "/home/index" || 
            path.StartsWith("/account/login") || path.StartsWith("/account/register"))
        {
            await _next(context);
            return;
        }

        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            await _next(context);
            return;
        }

        var sessionIdClaim = context.User.FindFirst("SessionId")?.Value;
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(sessionIdClaim) || string.IsNullOrEmpty(userIdClaim))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Session expired. Please login again.");
            return;
        }

        if (!Guid.TryParse(sessionIdClaim, out var sessionId) || !int.TryParse(userIdClaim, out var userId))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid session. Please login again.");
            return;
        }

        var isValid = await sessionService.ValidateSessionAsync(sessionId, userId);
        
        if (!isValid)
        {
            context.Response.StatusCode = 401;
            context.Response.Cookies.Delete("EInsurance.Token");
            await context.Response.WriteAsync("Session expired. Another login detected. Please login again.");
            return;
        }

        await _next(context);
    }
}