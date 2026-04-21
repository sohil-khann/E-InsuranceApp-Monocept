using EInsurance.Models.Policies;

namespace EInsurance.Interfaces;

public interface IPremiumCalculationService
{
    
    Task<PremiumCalculationResultViewModel> CalculatePremiumAsync(
        int schemeId,
        decimal sumAssured,
        int ageAtMaturity,
        int maturityPeriodMonths,
        CancellationToken cancellationToken = default);

    Task<List<SchemeCalculationViewModel>> GetSchemesWithCalculationDetailsAsync(
        int? planId = null,
        CancellationToken cancellationToken = default);
}
