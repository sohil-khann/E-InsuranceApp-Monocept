using EInsurance.Services.Authentication;

namespace EInsurance.Interfaces;

public interface IUserAuthenticationService
{
    Task<AuthenticatedUser?> AuthenticateAsync(string identifier, string password, CancellationToken cancellationToken = default);
    Task<AuthenticatedUser?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
}
