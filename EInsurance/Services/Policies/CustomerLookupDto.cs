namespace EInsurance.Interfaces;

public record CustomerLookupDto(
    int CustomerId,
    string FullName,
    string Email);
