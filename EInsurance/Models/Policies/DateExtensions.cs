namespace EInsurance.Models.Policies;

/// <summary>
/// Extension methods for date-related calculations
/// </summary>
public static class DateExtensions
{
    /// <summary>
    /// Calculates age from a DateOnly value
    /// </summary>
    public static int CalculateAge(this DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age))
            age--;

        return age;
    }
}
