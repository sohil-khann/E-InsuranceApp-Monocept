using System.ComponentModel.DataAnnotations;

namespace EInsurance.Models.Policies;


public class PremiumCalculationInputViewModel
{
    [Required]
    public int SchemeId { get; set; }

    [Required]
    public string SchemeName { get; set; } = string.Empty;

    [Required]
    [Range(1000, 10000000, ErrorMessage = "Sum Assured must be between 1,000 and 10,000,000")]
    [Display(Name = "Sum Assured (Coverage Amount)")]
    public decimal SumAssured { get; set; }

    [Display(Name = "Your Age")]
    [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
    public int? CustomerAge { get; set; }

    [Required]
    [Range(12, 360, ErrorMessage = "Maturity period must be between 12 and 360 months")]
    [Display(Name = "Maturity Period (Months)")]
    public int MaturityPeriodMonths { get; set; }

    [Display(Name = "Beneficiary Name")]
    [MaxLength(100)]
    public string? BeneficiaryName { get; set; }
}


public class PremiumCalculationResultViewModel
{
    public int SchemeId { get; set; }

    public string SchemeName { get; set; } = string.Empty;

    public string PlanName { get; set; } = string.Empty;

    [Display(Name = "Sum Assured")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal SumAssured { get; set; }

    [Display(Name = "Interest Rate")]
    [DisplayFormat(DataFormatString = "{0:P}")]
    public decimal InterestRate { get; set; }

    [Display(Name = "Risk Factor")]
    [DisplayFormat(DataFormatString = "{0:P}")]
    public decimal RiskFactor { get; set; }

    [Display(Name = "Maturity Period")]
    public int MaturityPeriodMonths { get; set; }

    [Display(Name = "Calculated Premium")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal CalculatedPremium { get; set; }

    [Display(Name = "Monthly Amount")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal MonthlyAmount { get; set; }

    public DateTime CalculatedAtUtc { get; set; }

   
    [Display(Name = "Quote ID")]
    public string QuoteId => $"Q-{SchemeId}-{DateOnly.FromDateTime(CalculatedAtUtc):yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
}

public class SchemeCalculationViewModel
{
    public int SchemeId { get; set; }

    public string SchemeName { get; set; } = string.Empty;

    public int PlanId { get; set; }

    public string PlanName { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:P}")]
    public decimal InterestRate { get; set; }

    public string SchemeDetails { get; set; } = string.Empty;
}

public class PremiumConfirmationViewModel
{
    public int SchemeId { get; set; }

    public string SchemeName { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal CalculatedPremium { get; set; }

    [Display(Name = "Maturity Period (Months)")]
    public int MaturityPeriodMonths { get; set; }

    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal SumAssured { get; set; }

    public string QuoteId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Display(Name = "Beneficiary Name")]
    public string BeneficiaryName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Payment Method")]
    public string PaymentMethod { get; set; } = string.Empty;
}
