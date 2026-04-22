namespace EInsurance.Models.Validation;

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public ValidationError() { }

    public ValidationError(string field, string message, string code)
    {
        Field = field;
        Message = message;
        Code = code;
    }
}

public class ValidationErrorResponse
{
    public bool Success => Errors.Count == 0;
    public string Message { get; set; } = "Validation failed";
    public List<ValidationError> Errors { get; set; } = new();
    public int StatusCode => 400;

    public ValidationErrorResponse() { }

    public ValidationErrorResponse(string message, List<ValidationError> errors)
    {
        Message = message;
        Errors = errors;
    }

    public static ValidationErrorResponse FromModelState(List<ValidationError> errors)
    {
        return new ValidationErrorResponse(errors.Count > 0 ? "Validation failed. Please correct the errors." : "Validation successful.", errors);
    }

    public static ValidationErrorResponse SingleError(string field, string message, string code = "INVALID")
    {
        return new ValidationErrorResponse("Validation failed.", new List<ValidationError> { new(field, message, code) });
    }
}