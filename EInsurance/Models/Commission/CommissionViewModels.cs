using EInsurance.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace EInsurance.Models.Commission;


public class AgentSummaryViewModel
{
    public int AgentId { get; set; }

    [Display(Name = "Agent Name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Active Policies")]
    public int ActivePoliciesCount { get; set; }

    [Display(Name = "Total Premium")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalPremium { get; set; }

    [Display(Name = "Commission Rate")]
    [DisplayFormat(DataFormatString = "{0:P}")]
    public decimal CommissionRate { get; set; }
}

public class CommissionCalculationInputViewModel
{
    [Required]
    [Display(Name = "Select Agent")]
    public int AgentId { get; set; }

    public string? AgentName { get; set; }

    [Required]
    [Display(Name = "From Date")]
    [DataType(DataType.Date)]
    public DateTime FromDate { get; set; } = DateTime.UtcNow.AddMonths(-1);

    [Required]
    [Display(Name = "To Date")]
    [DataType(DataType.Date)]
    public DateTime ToDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "Only Paid Policies")]
    public bool OnlyPaidPolicies { get; set; } = true;
}

public class CommissionCalculationResultViewModel
{
    public int AgentId { get; set; }

    public string AgentName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [Display(Name = "Calculation Period")]
    public string CalculationPeriod => $"{FromDate:MMM dd, yyyy} - {ToDate:MMM dd, yyyy}";

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    [Display(Name = "Total Policies")]
    public int TotalPoliciesCount { get; set; }

    [Display(Name = "Total Premium")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalPremium { get; set; }

    [Display(Name = "Commission Rate")]
    [DisplayFormat(DataFormatString = "{0:P}")]
    public decimal CommissionRate { get; set; }

    [Display(Name = "Total Commission")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalCommissionAmount { get; set; }

    [Display(Name = "Calculated At")]
    public DateTime CalculatedAtUtc { get; set; }

    public List<PolicyCommissionDetailViewModel> PolicyDetails { get; set; } = [];
}


public class PolicyCommissionDetailViewModel
{
    public int PolicyId { get; set; }

    [Display(Name = "Policy Number")]
    public string PolicyNumber { get; set; } = string.Empty;

    [Display(Name = "Customer")]
    public string CustomerName { get; set; } = string.Empty;

    [Display(Name = "Scheme")]
    public string SchemeName { get; set; } = string.Empty;

    [Display(Name = "Premium")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Premium { get; set; }

    [Display(Name = "Commission")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal CommissionAmount { get; set; }

    [Display(Name = "Issued Date")]
    [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
    public DateTime IssuedDate { get; set; }

    [Display(Name = "Status")]
    public string Status { get; set; } = "Active";
}

public class AgentCommissionDashboardViewModel
{
    public int AgentId { get; set; }

    public string AgentName { get; set; } = string.Empty;

    public string ProfileImageUrl { get; set; } = string.Empty;

    public string Role { get; set; } = "Senior Underwriter";

    public string Region { get; set; } = string.Empty;

    [Display(Name = "Performance Badge")]
    public string PerformanceBadge { get; set; } = "Top Performer";

  
    [Display(Name = "Active Policies (Current Month)")]
    public int ActivePoliciesCurrentMonth { get; set; }

    [Display(Name = "Month-over-Month Change")]
    [DisplayFormat(DataFormatString = "{0:P}")]
    public decimal MonthOverMonthChange { get; set; }

    [Display(Name = "Total Premium (Current Month)")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalPremiumCurrentMonth { get; set; }

    [Display(Name = "Commission Earned (Current Month)")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal CommissionEarnedCurrentMonth { get; set; }

    [Display(Name = "Commission Status")]
    public string CommissionStatus { get; set; } = "Paid & Finalized";

  
    [Display(Name = "Total Premium (Year to Date)")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalPremiumYTD { get; set; }

    [Display(Name = "Total Commission (Year to Date)")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalCommissionYTD { get; set; }

    
    public List<DisbursementViewModel> RecentDisbursements { get; set; } = [];

    
    public List<MonthlyPerformanceViewModel> MonthlyPerformance { get; set; } = [];
}


public class DisbursementViewModel
{
    public int CommissionId { get; set; }

    [Display(Name = "Policy ID")]
    public string PolicyReference { get; set; } = string.Empty;

    [Display(Name = "Policy Name")]
    public string PolicyName { get; set; } = string.Empty;

    [Display(Name = "Amount")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Amount { get; set; }

    [Display(Name = "Processed Date")]
    [DisplayFormat(DataFormatString = "{0:MMM dd}")]
    public DateTime ProcessedDate { get; set; }
}

/// <summary>
/// Monthly performance metrics for charting
/// </summary>
public class MonthlyPerformanceViewModel
{
    public string Month { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Commission { get; set; }
}


public class CommissionReportViewModel
{
    public int AgentId { get; set; }

    public string AgentName { get; set; } = string.Empty;

    [Display(Name = "Report Period")]
    public string ReportPeriod => $"{FromDate:MMM dd, yyyy} - {ToDate:MMM dd, yyyy}";

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    [Display(Name = "Total Policies")]
    public int TotalPoliciesCount { get; set; }

    [Display(Name = "Paid Policies")]
    public int PaidPoliciesCount { get; set; }

    [Display(Name = "Total Premium")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalPremium { get; set; }

    [Display(Name = "Commission Rate")]
    [DisplayFormat(DataFormatString = "{0:P}")]
    public decimal CommissionRate { get; set; }

    [Display(Name = "Total Commission")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalCommissionAmount { get; set; }

    [Display(Name = "Finalized At")]
    public DateTime FinalizedAtUtc { get; set; }

    public string Status { get; set; } = "Success";

    public List<PolicyCommissionDetailViewModel> PolicyDetails { get; set; } = [];
}

public class CommissionLedgerEntryViewModel
{
    public int CommissionId { get; set; }

    [Display(Name = "Policy")]
    public string PolicyNumber { get; set; } = string.Empty;

    [Display(Name = "Customer")]
    public string CustomerName { get; set; } = string.Empty;

    [Display(Name = "Premium")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Premium { get; set; }

    [Display(Name = "Commission")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal CommissionAmount { get; set; }

    [Display(Name = "Status")]
    public string Status { get; set; } = "Pending";

    [Display(Name = "Calculated Date")]
    [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
    public DateTime CalculatedDate { get; set; }

    [Display(Name = "Paid Date")]
    [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
    public DateTime? PaidDate { get; set; }
}

public class CommissionLedgerViewModel
{
    public int AgentId { get; set; }
    public PagedResult<CommissionLedgerEntryViewModel> Ledger { get; set; } = new();

    [Display(Name = "Total Commission")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal TotalCommissionAmount { get; set; }

    [Display(Name = "Paid Commissions")]
    public int PaidCount { get; set; }

    [Display(Name = "Pending")]
    public int PendingCount { get; set; }
}
