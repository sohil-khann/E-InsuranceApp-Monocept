using EInsurance.Data;
using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EInsurance.Services.Session;

public class SessionService(ApplicationDbContext dbContext) : ISessionService
{
    private static readonly TimeSpan SessionExpiry = TimeSpan.FromMinutes(30);

    public async Task<Guid> CreateSessionAsync(int userId, string userType, string deviceInfo, string? ipAddress, CancellationToken cancellationToken = default)
    {
        await InvalidateUserSessionsAsync(userId, cancellationToken);

        var session = new UserSession
        {
            SessionId = Guid.NewGuid(),
            UserId = userId,
            UserType = userType,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow,
            LastActiveAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(SessionExpiry),
            IsActive = true
        };

        dbContext.UserSessions.Add(session);
        await dbContext.SaveChangesAsync(cancellationToken);

        return session.SessionId;
    }

    public async Task<bool> ValidateSessionAsync(Guid sessionId, int userId, CancellationToken cancellationToken = default)
    {
        var session = await dbContext.UserSessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.UserId == userId && s.IsActive, cancellationToken);

        if (session == null)
        {
            return false;
        }

        if (session.ExpiresAt < DateTime.UtcNow)
        {
            session.IsActive = false;
            await dbContext.SaveChangesAsync(cancellationToken);
            return false;
        }

        session.LastActiveAt = DateTime.UtcNow;
        session.ExpiresAt = DateTime.UtcNow.Add(SessionExpiry);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> InvalidateSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await dbContext.UserSessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId, cancellationToken);

        if (session == null)
        {
            return false;
        }

        session.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> InvalidateUserSessionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var sessions = await dbContext.UserSessions
            .Where(s => s.UserId == userId && s.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.IsActive = false;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateSessionActivityAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await dbContext.UserSessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.IsActive, cancellationToken);

        if (session == null)
        {
            return false;
        }

        session.LastActiveAt = DateTime.UtcNow;
        session.ExpiresAt = DateTime.UtcNow.Add(SessionExpiry);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<Guid?> GetActiveSessionIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var session = await dbContext.UserSessions
            .Where(s => s.UserId == userId && s.IsActive && s.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.LastActiveAt)
            .FirstOrDefaultAsync(cancellationToken);

        return session?.SessionId;
    }
}