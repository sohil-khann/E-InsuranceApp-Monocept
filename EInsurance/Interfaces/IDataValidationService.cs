using EInsurance.Models.Validation;

namespace EInsurance.Interfaces;

public interface IDataValidationService
{
    Task<ValidationErrorResponse> ValidateEmailUniquenessAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<ValidationErrorResponse> ValidateUsernameUniquenessAsync(string username, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<ValidationErrorResponse> ValidateEmployeeSchemeIntegrityAsync(int employeeId, int schemeId, CancellationToken cancellationToken = default);
    Task<ValidationErrorResponse> ValidateCustomerAgeAsync(DateOnly dateOfBirth, CancellationToken cancellationToken = default);
    ValidationErrorResponse ValidateEmailFormat(string email);
    ValidationErrorResponse ValidateUsernameFormat(string username);
    ValidationErrorResponse ValidatePasswordFormat(string password);
    ValidationErrorResponse ValidatePhoneFormat(string phone);
    ValidationErrorResponse ValidateCurrencyFormat(decimal amount);
    ValidationErrorResponse ValidateRoleFormat(string role);
}