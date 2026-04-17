using System.ComponentModel.DataAnnotations;

namespace EInsurance.Models.Auth;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Username or email")]
    public string Identifier { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
