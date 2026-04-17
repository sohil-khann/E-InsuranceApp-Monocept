using EInsurance.Domain.Entities;

namespace EInsurance.Interfaces;

public interface IRefreshTokenService
{
    Task<string> GenerateRefreshTokenAsync(int userId, string ipAddress, CancellationToken cancellationToken = default);
    Task<bool> ValidateRefreshTokenAsync(string token, int userId, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
}
