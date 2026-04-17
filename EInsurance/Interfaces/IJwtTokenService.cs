using EInsurance.Services.Authentication;

namespace EInsurance.Interfaces;

public interface IJwtTokenService
{
    AuthenticationResult GenerateToken(AuthenticatedUser user);
}
