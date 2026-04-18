namespace EInsurance.Interfaces;

public record PaymentDetailsDto(
    int PaymentId,
    decimal Amount,
    DateOnly PaymentDate);
