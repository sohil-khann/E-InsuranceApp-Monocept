using EInsurance.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Text.Json;

namespace EInsurance.Controllers;

[Route("webhook")]
[ApiController]
public class StripeWebhookController : Controller
{
    private readonly IPolicyRepository _policyRepository;
    private readonly IStripePaymentService _stripePaymentService;
    private readonly ILogger<StripeWebhookController> _logger;
    private readonly string _webhookSecret;

    public StripeWebhookController(
        IPolicyRepository policyRepository,
        IStripePaymentService stripePaymentService,
        ILogger<StripeWebhookController> logger,
        IConfiguration configuration)
    {
        _policyRepository = policyRepository;
        _stripePaymentService = stripePaymentService;
        _logger = logger;
        _webhookSecret = configuration["Stripe:WebhookSecret"] 
            ?? throw new InvalidOperationException("Stripe WebhookSecret not configured");
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> HandleStripeWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _webhookSecret);

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    await HandlePaymentIntentSucceededAsync(stripeEvent);
                    break;

                case "payment_intent.payment_failed":
                    await HandlePaymentIntentFailedAsync(stripeEvent);
                    break;

                default:
                    _logger.LogInformation("Unhandled event type: {EventType}", stripeEvent.Type);
                    break;
            }

            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook error");
            return BadRequest(new { error = ex.Message });
        }
    }

    private async Task HandlePaymentIntentSucceededAsync(Event stripeEvent)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        if (paymentIntent == null) return;

        _logger.LogInformation(
            "Payment succeeded: {PaymentIntentId}, Amount: {Amount}",
            paymentIntent.Id,
            paymentIntent.Amount);

        await _policyRepository.UpdatePaymentStatusAsync(
            paymentIntent.Id,
            "succeeded",
            null);
    }

    private async Task HandlePaymentIntentFailedAsync(Event stripeEvent)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        if (paymentIntent == null) return;

        var failureMessage = paymentIntent.LastPaymentError?.Message ?? "Unknown error";

        _logger.LogWarning(
            "Payment failed: {PaymentIntentId}, Reason: {Reason}",
            paymentIntent.Id,
            failureMessage);

        await _policyRepository.UpdatePaymentStatusAsync(
            paymentIntent.Id,
            "failed",
            failureMessage);
    }
}