namespace EInsurance.Interfaces;

public interface ISessionService
{
    Task<Guid> CreateSessionAsync(int userId, string userType, string deviceInfo, string? ipAddress, CancellationToken cancellationToken = default);
    Task<bool> ValidateSessionAsync(Guid sessionId, int userId, CancellationToken cancellationToken = default);
    Task<bool> InvalidateSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<bool> InvalidateUserSessionsAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> UpdateSessionActivityAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<Guid?> GetActiveSessionIdAsync(int userId, CancellationToken cancellationToken = default);
}