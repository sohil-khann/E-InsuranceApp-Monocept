using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EInsurance.Security.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class StrongPasswordAttribute : ValidationAttribute
{
    public int MinimumLength { get; set; } = ValidationConstants.Password.MinLength;

    public StrongPasswordAttribute()
    {
        ErrorMessage = "Password must be at least {1} characters long and contain at least one special character.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var password = value as string;
        if (string.IsNullOrEmpty(password))
        {
            return new ValidationResult("Password is required.", new[] { validationContext.MemberName });
        }

        if (password.Length < MinimumLength)
        {
            return new ValidationResult(
                string.Format(CultureInfo.CurrentCulture, ErrorMessageString, validationContext.DisplayName, MinimumLength),
                new[] { validationContext.MemberName });
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
            return new ValidationResult(
                "Password must contain at least one special character (!@#$%^&*()_+-=[]{}|;:'\',./<>?).",
                new[] { validationContext.MemberName });
        }

        return ValidationResult.Success;
    }
}