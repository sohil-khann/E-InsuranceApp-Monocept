namespace EInsurance.Interfaces;

public record CustomerPoliciesDto(
    int CustomerId,
    string FullName,
    string Email,
    List<PolicyDetailsDto> Policies);
