namespace EInsurance.Interfaces;

public record CustomerPoliciesDto(
    int CustomerId,
    string FullName,
    string Email,
    DateOnly DateOfBirth,
    List<PolicyDetailsDto> Policies);
