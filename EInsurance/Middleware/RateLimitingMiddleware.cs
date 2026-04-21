using System.Collections.Concurrent;
using System.Net;

namespace EInsurance.Middleware;


public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly int _maxAttempts;
    private readonly int _windowMinutes;

    private static readonly ConcurrentDictionary<string, List<DateTime>> AttemptTracker = new();

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, int maxAttempts = 5, int windowMinutes = 15)
    {
        _next = next;
        _logger = logger;
        _maxAttempts = maxAttempts;
        _windowMinutes = windowMinutes;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (IsAuthenticationEndpoint(context.Request.Path))
        {
            var clientIp = GetClientIpAddress(context);
            var endpoint = context.Request.Path.ToString();
            var key = $"{clientIp}:{endpoint}";

            if (!IsRequestAllowed(key))
            {
                _logger.LogWarning($"Rate limit exceeded for IP {clientIp} on endpoint {endpoint}");
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            await RecordAttemptIfAuthenticationFailed(context, key);
        }

        await _next(context);
    }

    private bool IsAuthenticationEndpoint(PathString path)
    {
        var pathValue = path.Value?.ToLowerInvariant() ?? string.Empty;
        return pathValue.Contains("/account/login") || pathValue.Contains("/account/register");
    }

    private bool IsRequestAllowed(string key)
    {
        if (!AttemptTracker.TryGetValue(key, out var attempts))
        {
            return true; 
        }

        var now = DateTime.UtcNow;
        var recentAttempts = attempts
            .Where(t => (now - t).TotalMinutes < _windowMinutes)
            .ToList();

        if (recentAttempts.Count == 0)
        {
            AttemptTracker.TryRemove(key, out _);
            return true;
        }

        AttemptTracker[key] = recentAttempts;

        return recentAttempts.Count < _maxAttempts;
    }

    private async Task RecordAttemptIfAuthenticationFailed(HttpContext context, string key)
    {
        if (context.Request.Method == "POST")
        {
            var now = DateTime.UtcNow;

            AttemptTracker.AddOrUpdate(key,
                new List<DateTime> { now },
                (_, attempts) =>
                {
                    attempts.Add(now);
                    return attempts;
                });
        }

        await Task.CompletedTask;
    }

    private string GetClientIpAddress(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var ips = forwardedFor.ToString().Split(',');
            return ips.FirstOrDefault()?.Trim() ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
