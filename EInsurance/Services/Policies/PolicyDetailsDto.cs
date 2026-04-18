namespace EInsurance.Interfaces;

public record PolicyDetailsDto(
    int PolicyId,
    string PlanName,
    string SchemeName,
    string PolicyDetails,
    decimal Premium,
    DateOnly DateIssued,
    int MaturityPeriod,
    DateOnly PolicyLapseDate,
    List<PaymentDetailsDto> Payments);
