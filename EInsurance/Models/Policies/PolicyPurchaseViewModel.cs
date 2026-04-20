using System.ComponentModel.DataAnnotations;

namespace EInsurance.Models.Policies;

public class AvailableSchemesViewModel
{
    public List<SchemeListItemViewModel> Schemes { get; set; } = [];
}

public class SchemeListItemViewModel
{
    public int SchemeId { get; set; }
    public string SchemeName { get; set; } = string.Empty;
    public string SchemeDetails { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public decimal BasePremium { get; set; } = 50.00m; // Example base premium
}

public class PurchasePolicyViewModel
{
    [Required]
    public int SchemeId { get; set; }
    
    public string SchemeName { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 60, ErrorMessage = "Maturity period must be between 1 and 60 months")]
    [Display(Name = "Maturity Period (Months)")]
    public int MaturityPeriod { get; set; }

    [Required]
    [Range(100, 1000000, ErrorMessage = "Coverage amount must be between 100 and 1,000,000")]
    [Display(Name = "Coverage Amount")]
    public decimal CoverageAmount { get; set; }

    [Required]
    [Display(Name = "Beneficiary Name")]
    public string BeneficiaryName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Payment Method")]
    public string PaymentMethod { get; set; } = string.Empty;
}

public class PurchaseConfirmationViewModel
{
    public int PolicyId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public string SchemeName { get; set; } = string.Empty;
    public decimal PremiumPaid { get; set; }
    public DateOnly ExpiryDate { get; set; }
}
