using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EInsurance.Security.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class MinimumAgeAttribute : ValidationAttribute
{
    public int MinimumAge { get; set; } = ValidationConstants.Age.Minimum;

    public MinimumAgeAttribute()
    {
        ErrorMessage = "You must be at least {1} years old.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateOnly dateOfBirth)
        {
            return new ValidationResult("Invalid date format.", new[] { validationContext.MemberName });
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age))
        {
            age--;
        }

        if (age < MinimumAge)
        {
            return new ValidationResult(
                string.Format(CultureInfo.CurrentCulture, ErrorMessageString, validationContext.DisplayName, MinimumAge),
                new[] { validationContext.MemberName });
        }

        return ValidationResult.Success;
    }
}