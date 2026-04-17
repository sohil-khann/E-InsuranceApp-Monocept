namespace EInsurance.Services.Authentication;

public record AuthenticationResult(
    string Token,
    DateTime ExpiresAtUtc,
    AuthenticatedUser User);
