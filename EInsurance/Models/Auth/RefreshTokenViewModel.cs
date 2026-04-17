using System.ComponentModel.DataAnnotations;

namespace EInsurance.Models.Auth;

public class RefreshTokenViewModel
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenResponseViewModel
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
