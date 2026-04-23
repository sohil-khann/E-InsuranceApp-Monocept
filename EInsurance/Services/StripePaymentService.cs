using EInsurance.Interfaces;
using Stripe;

namespace EInsurance.Services;

public class StripePaymentService : IStripePaymentService
{
    private readonly StripeClient _stripeClient;

    public StripePaymentService(IConfiguration configuration)
    {
        var secretKey = configuration["Stripe:SecretKey"] 
            ?? throw new InvalidOperationException("Stripe SecretKey not configured");
        _stripeClient = new StripeClient(secretKey);
    }

    public async Task<CreatePaymentIntentResult> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, CancellationToken cancellationToken = default)
    {
        var customerRequest = new CreateCustomerRequest
        {
            Email = request.CustomerEmail ?? $"customer_{request.CustomerId}@example.com",
            Name = request.CustomerName ?? $"Customer {request.CustomerId}"
        };

        var customerResult = await CreateCustomerAsync(customerRequest, cancellationToken);

        var metadata = new Dictionary<string, string>
        {
            { "customerId", request.CustomerId.ToString() },
            { "policyId", request.PolicyId.ToString() }
        };

        if (!string.IsNullOrEmpty(request.PolicyNumber))
        {
            metadata["policyNumber"] = request.PolicyNumber;
        }

        var amountInSmallestUnit = (long)(request.Amount * 100);

        var paymentIntentCreateOptions = new PaymentIntentCreateOptions
        {
            Amount = amountInSmallestUnit,
            Currency = request.Currency.ToLowerInvariant(),
            Customer = customerResult.CustomerId,
            Description = request.Description ?? $"Insurance Premium Payment for Policy {request.PolicyNumber}",
            Metadata = metadata,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            }
        };

        var paymentIntentService = new PaymentIntentService(_stripeClient);
        var paymentIntent = await paymentIntentService.CreateAsync(paymentIntentCreateOptions, cancellationToken: cancellationToken);

        var ephemeralKeyOptions = new EphemeralKeyCreateOptions
        {
            Customer = customerResult.CustomerId,
            StripeVersion = "2023-10-16"
        };

        var ephemeralKeyService = new EphemeralKeyService(_stripeClient);
        var ephemeralKey = await ephemeralKeyService.CreateAsync(ephemeralKeyOptions, cancellationToken: cancellationToken);

        return new CreatePaymentIntentResult
        {
            PaymentIntentId = paymentIntent.Id,
            ClientSecret = paymentIntent.ClientSecret ?? string.Empty,
            CustomerId = customerResult.CustomerId,
            EphemeralKey = ephemeralKey.Secret
        };
    }

    public async Task<PaymentIntentResult?> GetPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
    {
        var paymentIntentService = new PaymentIntentService(_stripeClient);

        try
        {
            var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

            return new PaymentIntentResult
            {
                Id = paymentIntent.Id,
                Status = paymentIntent.Status ?? "unknown",
                Amount = paymentIntent.Amount / 100m,
                Currency = paymentIntent.Currency ?? "inr",
                ClientSecret = paymentIntent.ClientSecret
            };
        }
        catch (StripeException)
        {
            return null;
        }
    }

    public async Task<bool> ConfirmPaymentIntentAsync(string paymentIntentId, string paymentMethodId, CancellationToken cancellationToken = default)
    {
        var paymentIntentService = new PaymentIntentService(_stripeClient);

        try
        {
            var paymentIntentUpdateOptions = new PaymentIntentUpdateOptions
            {
                PaymentMethod = paymentMethodId
            };

            await paymentIntentService.UpdateAsync(paymentIntentId, paymentIntentUpdateOptions, cancellationToken: cancellationToken);

            var paymentIntent = await paymentIntentService.ConfirmAsync(paymentIntentId, cancellationToken: cancellationToken);

            return paymentIntent.Status == "succeeded";
        }
        catch (StripeException)
        {
            return false;
        }
    }

    public async Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customerService = new CustomerService(_stripeClient);

        var customerCreateOptions = new CustomerCreateOptions
        {
            Email = request.Email,
            Name = request.Name,
            Metadata = new Dictionary<string, string>
            {
                { "source", "EInsurance" }
            }
        };

        var customer = await customerService.CreateAsync(customerCreateOptions, cancellationToken: cancellationToken);

        return new CreateCustomerResult
        {
            CustomerId = customer.Id
        };
    }
}