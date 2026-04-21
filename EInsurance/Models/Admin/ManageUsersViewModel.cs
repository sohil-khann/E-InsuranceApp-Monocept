using System.ComponentModel.DataAnnotations;

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
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;
}

public class EditUserViewModel
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
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
