using System.ComponentModel.DataAnnotations;
using EInsurance.Security;
using EInsurance.Security.Validation;

namespace EInsurance.Models.Auth;

public class EmployeeRegistrationViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [MinLength(ValidationConstants.FullName.MinLength, ErrorMessage = "Full name must be at least 2 characters.")]
    [MaxLength(ValidationConstants.FullName.MaxLength, ErrorMessage = "Full name must not exceed 100 characters.")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [MaxLength(ValidationConstants.Email.MaxLength, ErrorMessage = "Email must not exceed 100 characters.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required.")]
    [MinLength(ValidationConstants.Username.MinLength, ErrorMessage = "Username must be at least 3 characters.")]
    [MaxLength(ValidationConstants.Username.MaxLength, ErrorMessage = "Username must not exceed 50 characters.")]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required.")]
    [ValidRole]
    [Display(Name = "Role")]
    public string Role { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StrongPassword]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}