using EInsurance.Interfaces;
using EInsurance.Models.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EInsurance.Controllers;

[Authorize(Roles = "Customer")]
[Route("stripe")]
public class StripeCheckoutController : Controller
{
    private readonly IStripePaymentService _stripePaymentService;
    private readonly IPolicyRepository _policyRepository;
    private readonly ILogger<StripeCheckoutController> _logger;

    public StripeCheckoutController(
        IStripePaymentService stripePaymentService,
        IPolicyRepository policyRepository,
        ILogger<StripeCheckoutController> logger)
    {
        _stripePaymentService = stripePaymentService;
        _policyRepository = policyRepository;
        _logger = logger;
    }

    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] Interfaces.CreatePaymentIntentRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var customerId))
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            request.CustomerId = customerId;

            var result = await _stripePaymentService.CreatePaymentIntentAsync(request);

            // Don't create payment record here - create it after payment succeeds
            // The payment will be created in ProcessPaymentComplete after Stripe confirms payment

            return Ok(new
            {
                clientSecret = result.ClientSecret,
                paymentIntentId = result.PaymentIntentId,
                ephemeralKey = result.EphemeralKey,
                customerId = result.CustomerId,
                amount = request.Amount,
                policyId = request.PolicyId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment intent");
            return StatusCode(500, new { error = "Failed to create payment intent" });
        }
    }

    [HttpGet("get-payment-status/{paymentIntentId}")]
    public async Task<IActionResult> GetPaymentStatus(string paymentIntentId)
    {
        try
        {
            var result = await _stripePaymentService.GetPaymentIntentAsync(paymentIntentId);

            if (result == null)
            {
                return NotFound(new { error = "Payment intent not found" });
            }

            return Ok(new
            {
                status = result.Status,
                amount = result.Amount,
                currency = result.Currency
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment status");
            return StatusCode(500, new { error = "Failed to get payment status" });
        }
    }

    [HttpGet("publishable-key")]
    public IActionResult GetPublishableKey()
    {
        var publishableKey = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY")
            ?? Request.HttpContext.Items["Stripe:PublishableKey"] as string
            ?? "pk_test_placeholder";

        return Ok(new { key = publishableKey });
    }
}