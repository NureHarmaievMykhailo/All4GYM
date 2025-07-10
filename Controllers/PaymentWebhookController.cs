using All4GYM.Data;
using All4GYM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

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

        Console.WriteLine("📥 Webhook received. Raw body:");
        Console.WriteLine(json);

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signature, secret);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Stripe webhook signature error: {ex.Message}");
            return BadRequest($"⚠️ Webhook error: {ex.Message}");
        }

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;

            Console.WriteLine($"✅ Stripe event: {stripeEvent.Type}");
            Console.WriteLine($"📧 Session Email: {session?.CustomerEmail}");

            if (session?.CustomerEmail == null)
            {
                Console.WriteLine("❌ Stripe session has no email.");
                return BadRequest("❌ Stripe session has no email.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == session.CustomerEmail);
            if (user == null)
            {
                Console.WriteLine($"❌ User with email {session.CustomerEmail} not found.");
                return BadRequest("❌ Користувача не знайдено");
            }

            var tier = ExtractTierFromSession(session);
            Console.WriteLine($"📦 Subscription tier from session: {tier}");

            var existing = await _context.Subscriptions
                .Where(s => s.UserId == user.Id && s.IsActive)
                .ToListAsync();

            foreach (var sub in existing)
            {
                sub.IsActive = false;
                Console.WriteLine($"➡️ Subscription {sub.Id} marked inactive for user {user.Email}");
            }

            var now = DateTime.UtcNow;
            var subscription = new All4GYM.Models.Subscription
            {
                UserId = user.Id,
                Type = tier.ToString(),
                StartDate = now,
                EndDate = now.AddMonths(1),
                IsActive = true
            };

            _context.Subscriptions.Add(subscription);
            user.HasActiveSubscription = true;
            user.SubscriptionTier = tier;

            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ New subscription {tier} saved for user {user.Email}");
        }

        return Ok();
    }

    private SubscriptionTier ExtractTierFromSession(Session session)
    {
        if (session.Metadata.TryGetValue("tier", out var tierValue))
        {
            Console.WriteLine($"🔍 Extracted tier from metadata: {tierValue}");
            return tierValue.ToLower() switch
            {
                "basic" => SubscriptionTier.Basic,
                "pro" => SubscriptionTier.Pro,
                "premium" => SubscriptionTier.Premium,
                _ => SubscriptionTier.Basic
            };
        }

        Console.WriteLine("⚠️ No tier found in session metadata. Defaulting to Basic.");
        return SubscriptionTier.Basic;
    }
}
