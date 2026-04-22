using EInsurance.Models.Common;

namespace EInsurance.Models.Policies;

public class AdminPoliciesViewModel
{
    public PagedResult<PolicyListItemViewModel> Policies { get; set; } = new();
    public string SearchTerm { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "dateissued";
    public bool SortDescending { get; set; } = true;
}

public class PolicyListItemViewModel
{
    public int PolicyId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string SchemeName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public decimal Premium { get; set; }
    public DateOnly DateIssued { get; set; }
    public int MaturityPeriod { get; set; }
    public DateOnly PolicyLapseDate { get; set; }
    public DateTime CreatedAt { get; set; }
}