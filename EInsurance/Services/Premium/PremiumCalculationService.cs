using EInsurance.Data;
using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using EInsurance.Models.Policies;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EInsurance.Services.Premium;


public class PremiumCalculationService(ApplicationDbContext dbContext) : IPremiumCalculationService
{
    private const decimal DefaultInterestRate = 0.05m; // 5% default
    private const decimal RiskFactorBase = 0.02m; // 2% base risk factor

   
    public async Task<PremiumCalculationResultViewModel> CalculatePremiumAsync(
        int schemeId,
        decimal sumAssured,
        int ageAtMaturity,
        int maturityPeriodMonths,
        CancellationToken cancellationToken = default)
    {
        var scheme = await dbContext.Schemes
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.SchemeId == schemeId, cancellationToken);

        if (scheme == null)
        {
            throw new InvalidOperationException($"Scheme with ID {schemeId} not found.");
        }

        var interestRate = ExtractInterestRateFromScheme(scheme.SchemeDetails);

        var riskFactor = CalculateRiskFactor(ageAtMaturity);

        var totalPremium = CalculateTotalPremium(sumAssured, interestRate, riskFactor, maturityPeriodMonths);

        totalPremium = ApplySchemeAdjustments(totalPremium, scheme.SchemeDetails);

        return new PremiumCalculationResultViewModel
        {
            SchemeId = schemeId,
            SchemeName = scheme.SchemeName,
            PlanName = scheme.Plan.PlanName,
            SumAssured = sumAssured,
            InterestRate = interestRate,
            RiskFactor = riskFactor,
            MaturityPeriodMonths = maturityPeriodMonths,
            CalculatedPremium = totalPremium,
            MonthlyAmount = totalPremium / maturityPeriodMonths,
            CalculatedAtUtc = DateTime.UtcNow
        };
    }

    public async Task<List<SchemeCalculationViewModel>> GetSchemesWithCalculationDetailsAsync(
        int? planId = null,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Schemes
            .Include(s => s.Plan)
            .AsQueryable();

        if (planId.HasValue)
        {
            query = query.Where(s => s.PlanId == planId);
        }

        var schemes = await query.ToListAsync(cancellationToken);

        return schemes.Select(s => new SchemeCalculationViewModel
        {
            SchemeId = s.SchemeId,
            SchemeName = s.SchemeName,
            PlanId = s.PlanId,
            PlanName = s.Plan.PlanName,
            InterestRate = ExtractInterestRateFromScheme(s.SchemeDetails),
            SchemeDetails = s.SchemeDetails
        }).ToList();
    }

    
    private decimal ExtractInterestRateFromScheme(string schemeDetailsJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(schemeDetailsJson);
            var root = doc.RootElement;

            // Try to find "interestRate" or "rate" property
            if (root.TryGetProperty("interestRate", out var rateElement) ||
                root.TryGetProperty("rate", out rateElement))
            {
                if (rateElement.TryGetDecimal(out var rate))
                {
                    // If stored as percentage (e.g., 5), convert to decimal (0.05)
                    return rate > 1 ? rate / 100 : rate;
                }
            }

            return DefaultInterestRate;
        }
        catch
        {
            return DefaultInterestRate;
        }
    }

 
    private decimal CalculateRiskFactor(int age)
    {
        decimal riskFactor = RiskFactorBase;

        
        if (age >= 60)
            riskFactor += 0.15m;
        else if (age >= 50)
            riskFactor += 0.10m;
        else if (age >= 40)
            riskFactor += 0.05m;
        else if (age < 25)
            riskFactor -= 0.02m; 

        return Math.Min(riskFactor, 0.35m);
    }

    private decimal CalculateTotalPremium(decimal sumAssured, decimal interestRate, decimal riskFactor, int maturityPeriodMonths)
    {
        if (maturityPeriodMonths <= 0)
            throw new ArgumentException("Maturity period must be greater than 0");

        var totalFactor = interestRate + riskFactor;
        var premium = (sumAssured * totalFactor) / maturityPeriodMonths;

      
        return Math.Round(premium, 2);
    }

   
    private decimal ApplySchemeAdjustments(decimal basePremium, string schemeDetailsJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(schemeDetailsJson);
            var root = doc.RootElement;

           
            if (root.TryGetProperty("discount", out var discountElement) &&
                discountElement.TryGetDecimal(out var discount))
            {
                var discountPercentage = discount > 1 ? discount / 100 : discount;
                basePremium = basePremium * (1 - discountPercentage);
            }

        
            if (root.TryGetProperty("surcharge", out var surchargeElement) &&
                surchargeElement.TryGetDecimal(out var surcharge))
            {
                var surchargePercentage = surcharge > 1 ? surcharge / 100 : surcharge;
                basePremium = basePremium * (1 + surchargePercentage);
            }

            return Math.Round(basePremium, 2);
        }
        catch
        {
            return basePremium;
        }
    }
}
