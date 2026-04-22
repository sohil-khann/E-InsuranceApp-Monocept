using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EInsurance.Security.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ValidRoleAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var role = value as string;
        if (string.IsNullOrEmpty(role))
        {
            return new ValidationResult("Role is required.", new[] { validationContext.MemberName });
        }

        if (!ValidationConstants.Role.ValidRoles.Contains(role))
        {
            return new ValidationResult(
                string.Format(CultureInfo.CurrentCulture, "Invalid role. Must be one of: {0}", string.Join(", ", ValidationConstants.Role.ValidRoles)),
                new[] { validationContext.MemberName });
        }

        return ValidationResult.Success;
    }
}