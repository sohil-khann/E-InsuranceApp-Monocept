namespace EInsurance.Services.Authentication;

public record AuthenticatedUser(
    int UserId,
    string DisplayName,
    string Email,
    string Role);
