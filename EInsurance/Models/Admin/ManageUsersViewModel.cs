using System.ComponentModel.DataAnnotations;
using EInsurance.Security;
using EInsurance.Security.Validation;

namespace EInsurance.Models.Admin;

public class ManageUsersViewModel
{
    public string SearchTerm { get; set; } = string.Empty;
    public List<UserListItemViewModel> Users { get; set; } = [];
}

public class UserListItemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = "Active"; 
    public DateTime CreatedAt { get; set; }
    public string? AgentName { get; set; }
}

public class CreateUserViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    [MinLength(ValidationConstants.FullName.MinLength, ErrorMessage = "Full name must be at least 2 characters.")]
    [MaxLength(ValidationConstants.FullName.MaxLength, ErrorMessage = "Full name must not exceed 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(ValidationConstants.Email.MaxLength, ErrorMessage = "Email must not exceed 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(ValidationConstants.Username.MaxLength, MinimumLength = ValidationConstants.Username.MinLength)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StrongPassword]
    public string Password { get; set; } = string.Empty;

    [Required]
    [ValidRole]
    public string Role { get; set; } = string.Empty;
}

public class EditUserViewModel
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Full Name")]
    [MinLength(ValidationConstants.FullName.MinLength, ErrorMessage = "Full name must be at least 2 characters.")]
    [MaxLength(ValidationConstants.FullName.MaxLength, ErrorMessage = "Full name must not exceed 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(ValidationConstants.Email.MaxLength, ErrorMessage = "Email must not exceed 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [ValidRole]
    public string Role { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = string.Empty;
}

public class AssignAgentViewModel
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    
    [Display(Name = "Current Agent")]
    public int? CurrentAgentId { get; set; }
    public string? CurrentAgentName { get; set; }
    
    [Display(Name = "Assign New Agent")]
    public int NewAgentId { get; set; }
    
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Agents { get; set; } = [];
}
