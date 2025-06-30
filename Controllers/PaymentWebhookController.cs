using All4GYM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace All4GYM.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentWebhookController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public PaymentWebhookController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    /// <summary>
    /// Webhook Stripe. Оновлює статус платежу в БД.
    /// </summary>
    /// <returns>200 OK</returns>
    [HttpPost]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Webhook від Stripe",
        Description = "Цей ендпоінт приймає події Stripe (наприклад, payment_intent.succeeded) і оновлює статус платежу в БД."
    )]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        var stripeSignature = Request.Headers["Stripe-Signature"];
        var endpointSecret = _config["Stripe:WebhookSecret"]; // appsettings.json

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, endpointSecret);
        }
        catch (Exception e)
        {
            return BadRequest($"⚠️ Webhook error: {e.Message}");
        }

        if (stripeEvent.Type == "payment_intent.succeeded")
        {
            var intent = stripeEvent.Data.Object as PaymentIntent;

            if (intent is not null)
            {
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.StripePaymentId == intent.Id);

                if (payment is not null)
                {
                    payment.Status = "Succeeded";
                    await _context.SaveChangesAsync();
                }
            }
        }

        return Ok();
    }
}