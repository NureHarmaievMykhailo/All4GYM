using All4GYM.Data;
using All4GYM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Stripe.Events;

namespace All4GYM.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StripeWebhookController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public StripeWebhookController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> HandleStripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"];
        var secret = _config["Stripe:WebhookSecret"];

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signature, secret);
        }
        catch (Exception ex)
        {
            return BadRequest($"⚠️ Webhook error: {ex.Message}");
        }

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;
            if (session != null && session.CustomerEmail != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == session.CustomerEmail);
                if (user != null)
                {
                    user.HasActiveSubscription = true;
                    user.SubscriptionTier = ExtractTierFromPriceId(session); // реалізуй цю функцію
                    await _context.SaveChangesAsync();
                }
            }
        }

        return Ok();
    }

    private SubscriptionTier ExtractTierFromPriceId(Session session)
    {
        if (session.Metadata.TryGetValue("tier", out var tierValue))
        {
            return tierValue.ToLower() switch
            {
                "basic" => SubscriptionTier.Basic,
                "pro" => SubscriptionTier.Pro,
                "premium" => SubscriptionTier.Premium,
                _ => SubscriptionTier.Basic
            };
        }

        // Якщо немає метаданих — резервний варіант за замовчуванням
        return SubscriptionTier.Basic;
    }

}
