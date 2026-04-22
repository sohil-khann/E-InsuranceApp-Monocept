using System.ComponentModel.DataAnnotations;
using EInsurance.Security;
using EInsurance.Security.Validation;

namespace EInsurance.Models.Auth;

public class CustomerRegistrationViewModel
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

    [Required(ErrorMessage = "Phone is required.")]
    [RegularExpression(ValidationConstants.Phone.RegexPattern, ErrorMessage = "Phone must be numeric and between 10-15 digits.")]
    [Display(Name = "Phone")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required.")]
    [DataType(DataType.Date)]
    [MinimumAge]
    [Display(Name = "Date of birth")]
    public DateOnly DateOfBirth { get; set; }

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

    [Display(Name = "Agent")]
    public int? AgentId { get; set; }
}