using System.Text.RegularExpressions;
using EInsurance.Data;
using EInsurance.Domain.Entities;
using EInsurance.Interfaces;
using EInsurance.Models.Validation;
using EInsurance.Security;
using Microsoft.EntityFrameworkCore;

namespace EInsurance.Services.Validation;

public class DataValidationService(ApplicationDbContext dbContext) : IDataValidationService
{
    public async Task<ValidationErrorResponse> ValidateEmailUniquenessAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        var emailLower = email.ToLowerInvariant();

        var adminExists = await dbContext.Admins.AnyAsync(a => a.Email.ToLower() == emailLower, cancellationToken);
        if (adminExists)
        {
            errors.Add(new ValidationError("Email", "Email is already in use by an Admin.", "DUPLICATE_EMAIL"));
        }

        var employeeExists = await dbContext.Employees.AnyAsync(e => e.Email.ToLower() == emailLower, cancellationToken);
        if (employeeExists)
        {
            errors.Add(new ValidationError("Email", "Email is already in use by an Employee.", "DUPLICATE_EMAIL"));
        }

        var agentExists = await dbContext.InsuranceAgents.AnyAsync(a => a.Email.ToLower() == emailLower, cancellationToken);
        if (agentExists)
        {
            errors.Add(new ValidationError("Email", "Email is already in use by an Insurance Agent.", "DUPLICATE_EMAIL"));
        }

        var customerExists = await dbContext.Customers.AnyAsync(c => c.Email.ToLower() == emailLower, cancellationToken);
        if (customerExists)
        {
            errors.Add(new ValidationError("Email", "Email is already in use by a Customer.", "DUPLICATE_EMAIL"));
        }

        return errors.Count > 0
            ? new ValidationErrorResponse("Email validation failed.", errors)
            : ValidationErrorResponse.FromModelState(new List<ValidationError>());
    }

    public async Task<ValidationErrorResponse> ValidateUsernameUniquenessAsync(string username, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        var usernameLower = username.ToLowerInvariant();

        var adminExists = await dbContext.Admins.AnyAsync(a => a.Username.ToLower() == usernameLower, cancellationToken);
        if (adminExists)
        {
            errors.Add(new ValidationError("Username", "Username is already in use by an Admin.", "DUPLICATE_USERNAME"));
        }

        var employeeExists = await dbContext.Employees.AnyAsync(e => e.Username.ToLower() == usernameLower, cancellationToken);
        if (employeeExists)
        {
            errors.Add(new ValidationError("Username", "Username is already in use by an Employee.", "DUPLICATE_USERNAME"));
        }

        var agentExists = await dbContext.InsuranceAgents.AnyAsync(a => a.Username.ToLower() == usernameLower, cancellationToken);
        if (agentExists)
        {
            errors.Add(new ValidationError("Username", "Username is already in use by an Insurance Agent.", "DUPLICATE_USERNAME"));
        }

        return errors.Count > 0
            ? new ValidationErrorResponse("Username validation failed.", errors)
            : ValidationErrorResponse.FromModelState(new List<ValidationError>());
    }

    public async Task<ValidationErrorResponse> ValidateEmployeeSchemeIntegrityAsync(int employeeId, int schemeId, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        var employeeExists = await dbContext.Employees.AnyAsync(e => e.EmployeeId == employeeId, cancellationToken);
        if (!employeeExists)
        {
            errors.Add(new ValidationError("EmployeeID", $"Employee with ID {employeeId} does not exist.", "INVALID_EMPLOYEE"));
        }

        var schemeExists = await dbContext.Schemes.AnyAsync(s => s.SchemeId == schemeId, cancellationToken);
        if (!schemeExists)
        {
            errors.Add(new ValidationError("SchemeID", $"Scheme with ID {schemeId} does not exist.", "INVALID_SCHEME"));
        }

        return errors.Count > 0
            ? new ValidationErrorResponse("EmployeeScheme validation failed.", errors)
            : ValidationErrorResponse.FromModelState(new List<ValidationError>());
    }

    public async Task<ValidationErrorResponse> ValidateCustomerAgeAsync(DateOnly dateOfBirth, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age))
        {
            age--;
        }

        if (age < ValidationConstants.Age.Minimum)
        {
            return ValidationErrorResponse.SingleError(
                "DateOfBirth",
                $"Customer must be at least {ValidationConstants.Age.Minimum} years old.",
                "INVALID_AGE");
        }

        return ValidationErrorResponse.FromModelState(new List<ValidationError>());
    }

    public ValidationErrorResponse ValidateEmailFormat(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return ValidationErrorResponse.SingleError("Email", "Email is required.", "REQUIRED_EMAIL");
        }

        if (email.Length > ValidationConstants.Email.MaxLength)
        {
            return ValidationErrorResponse.SingleError("Email", $"Email must not exceed {ValidationConstants.Email.MaxLength} characters.", "EMAIL_TOO_LONG");
        }

        var emailRegex = new Regex(ValidationConstants.Email.RegexPattern, RegexOptions.IgnoreCase);
        if (!emailRegex.IsMatch(email))
        {
            return ValidationErrorResponse.SingleError("Email", "Invalid email format.", "INVALID_EMAIL_FORMAT");
        }

        return ValidationErrorResponse.FromModelState(new List<ValidationError>());
    }

    public ValidationErrorResponse ValidateUsernameFormat(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return ValidationErrorResponse.SingleError("Username", "Username is required.", "REQUIRED_USERNAME");
        }

        if (username.Length < ValidationConstants.Username.MinLength)
        {
            return ValidationErrorResponse.SingleError("Username", $"Username must be at least {ValidationConstants.Username.MinLength} characters.", "USERNAME_TOO_SHORT");
        }

        if (username.Length > ValidationConstants.Username.MaxLength)
        {
            return ValidationErrorResponse.SingleError("Username", $"Username must not exceed {ValidationConstants.Username.MaxLength} characters.", "USERNAME_TOO_LONG");
        }

        return ValidationErrorResponse.FromModelState(new List<ValidationError>());
    }

    public ValidationErrorResponse ValidatePasswordFormat(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return ValidationErrorResponse.SingleError("Password", "Password is required.", "REQUIRED_PASSWORD");
        }

        if (password.Length < ValidationConstants.Password.MinLength)
        {
            return ValidationErrorResponse.SingleError("Password", $"Password must be at least {ValidationConstants.Password.MinLength} characters.", "PASSWORD_TOO_SHORT");
        }

        var hasSpecialChar = false;
        foreach (var c in password)
        {
            if (ValidationConstants.Password.SpecialCharacters.Contains(c))
            {
                hasSpecialChar = true;
                break;
            }
        }

        if (!hasSpecialChar)
        {
            return ValidationErrorResponse.SingleError("Password", "Password must contain at least one special character.", "WEAK_PASSWORD");
        }

        return ValidationErrorResponse.FromModelState(new List<ValidationError>());
    }

    public ValidationErrorResponse ValidatePhoneFormat(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return ValidationErrorResponse.SingleError("Phone", "Phone is required.", "REQUIRED_PHONE");
        }

        var phoneRegex = new Regex(ValidationConstants.Phone.RegexPattern);
        if (!phoneRegex.IsMatch(phone))
        {
            return ValidationErrorResponse.SingleError("Phone", $"Phone must be numeric and between {ValidationConstants.Phone.MinLength}-{ValidationConstants.Phone.MaxLength} digits.", "INVALID_PHONE_FORMAT");
        }

        return ValidationErrorResponse.FromModelState(new List<ValidationError>());
    }

    public ValidationErrorResponse ValidateCurrencyFormat(decimal amount)
    {
        if (amount < 0)
        {
            return ValidationErrorResponse.SingleError("Amount", "Amount cannot be negative.", "NEGATIVE_AMOUNT");
        }

        var rounded = Math.Round(amount, 2);
        var difference = Math.Abs(amount - rounded);

        if (difference > 0.001m)
        {
            return ValidationErrorResponse.SingleError(
                "Amount",
                $"Amount must have at most 2 decimal places.",
                "INVALID_CURRENCY_FORMAT");
        }

        return ValidationErrorResponse.FromModelState(new List<ValidationError>());
    }

    public ValidationErrorResponse ValidateRoleFormat(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return ValidationErrorResponse.SingleError("Role", "Role is required.", "REQUIRED_ROLE");
        }

        if (!ValidationConstants.Role.ValidRoles.Contains(role))
        {
            return ValidationErrorResponse.SingleError(
                "Role",
                $"Role must be one of: {string.Join(", ", ValidationConstants.Role.ValidRoles)}",
                "INVALID_ROLE");
        }

        return ValidationErrorResponse.FromModelState(new List<ValidationError>());
    }
}