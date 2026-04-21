using EInsurance.Configuration;
using EInsurance.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EInsurance.Services.Authentication;

public class JwtTokenService(IOptions<JwtSettings> jwtOptions) : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public AuthenticationResult GenerateToken(AuthenticatedUser user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new ArgumentException("User email cannot be null or empty", nameof(user.Email));
        }

        if (string.IsNullOrWhiteSpace(user.DisplayName))
        {
            throw new ArgumentException("User display name cannot be null or empty", nameof(user.DisplayName));
        }

        if (string.IsNullOrWhiteSpace(user.Role))
        {
            throw new ArgumentException("User role cannot be null or empty", nameof(user.Role));
        }

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthenticationResult(tokenValue, expiresAtUtc, user);
    }
}
