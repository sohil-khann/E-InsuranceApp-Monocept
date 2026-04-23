namespace EInsurance.Interfaces;

public interface IStripePaymentService
{
    Task<CreatePaymentIntentResult> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, CancellationToken cancellationToken = default);
    Task<PaymentIntentResult?> GetPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
    Task<bool> ConfirmPaymentIntentAsync(string paymentIntentId, string paymentMethodId, CancellationToken cancellationToken = default);
    Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
}

public class CreatePaymentIntentRequest
{
    public int CustomerId { get; set; }
    public int PolicyId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "inr";
    public string? CustomerEmail { get; set; }
    public string? CustomerName { get; set; }
    public string? PolicyNumber { get; set; }
    public string? Description { get; set; }
}

public class CreatePaymentIntentResult
{
    public string PaymentIntentId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string? EphemeralKey { get; set; }
}

public class PaymentIntentResult
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? ClientSecret { get; set; }
}

public class CreateCustomerRequest
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class CreateCustomerResult
{
    public string CustomerId { get; set; } = string.Empty;
}