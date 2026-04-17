namespace EInsurance.Services.Registration;

public record RegistrationResult(bool Succeeded, string Message)
{
    public static RegistrationResult Success(string message) => new(true, message);

    public static RegistrationResult Failure(string message) => new(false, message);
}
