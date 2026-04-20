using EInsurance.Models.Policies;

namespace EInsurance.Interfaces;

/// <summary>
/// Service for calculating insurance premiums based on scheme details and customer information.
/// Implements UC-5: Premium Calculation (Schema-Integrated)
/// </summary>
public interface IPremiumCalculationService
{
    /// <summary>
    /// Calculates premium for a given scheme, sum assured, age, and maturity period.
    /// </summary>
    /// <param name="schemeId">The scheme ID for which to calculate premium</param>
    /// <param name="sumAssured">The sum assured (coverage amount) by the customer</param>
    /// <param name="ageAtMaturity">Customer's age at policy maturity</param>
    /// <param name="maturityPeriodMonths">Maturity period in months</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Premium calculation result with breakdown</returns>
    Task<PremiumCalculationResultViewModel> CalculatePremiumAsync(
        int schemeId,
        decimal sumAssured,
        int ageAtMaturity,
        int maturityPeriodMonths,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available schemes with their calculation details.
    /// </summary>
    /// <param name="planId">Optional filter by plan ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of schemes with calculation details</returns>
    Task<List<SchemeCalculationViewModel>> GetSchemesWithCalculationDetailsAsync(
        int? planId = null,
        CancellationToken cancellationToken = default);
}
