using EInsurance.Data;
using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace EInsurance.Services.Authentication;

public class RefreshTokenService(ApplicationDbContext dbContext) : IRefreshTokenService
{
    private const int RefreshTokenExpiryDays = 7;

    public async Task<string> GenerateRefreshTokenAsync(int userId, string ipAddress, CancellationToken cancellationToken = default)
    {
        var tokenValue = GenerateSecureRandomToken();

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = tokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays),
            CreatedAtUtc = DateTime.UtcNow,
            IpAddress = ipAddress
        };

        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return tokenValue;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token, int userId, CancellationToken cancellationToken = default)
    {
        var refreshToken = await GetRefreshTokenAsync(token, cancellationToken);

        if (refreshToken == null)
            return false;

        // Check if token belongs to the user
        if (refreshToken.UserId != userId)
            return false;

        // Check if token is still valid
        if (!refreshToken.IsValid)
            return false;

        return true;
    }

    public async Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var refreshToken = await GetRefreshTokenAsync(token, cancellationToken);

        if (refreshToken != null && !refreshToken.IsRevoked)
        {
            refreshToken.RevokedAtUtc = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    private static string GenerateSecureRandomToken()
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
