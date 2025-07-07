using Stripe;
using Stripe.Checkout;
using Microsoft.Extensions.Configuration;

namespace All4GYM.Services.Stripe;

public class StripePaymentIntentService
{
    private readonly IConfiguration _configuration;

    public StripePaymentIntentService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<PaymentIntent> CreateAsync(long amountInCents, string currency, string orderId)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = amountInCents,
            Currency = currency,
            Metadata = new Dictionary<string, string>
            {
                { "order_id", orderId }
            },
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            }
        };

        var service = new PaymentIntentService();
        return await service.CreateAsync(options);
    }

    public async Task<string> CreateCheckoutSessionAsync(string userEmail, string tier, string successUrl, string cancelUrl)
    {
        var priceId = tier.ToLower() switch
        {
            "basic" => _configuration["Stripe:PriceIds:Basic"],
            "pro" => _configuration["Stripe:PriceIds:Pro"],
            "premium" => _configuration["Stripe:PriceIds:Premium"],
            _ => throw new ArgumentException("Невірний тип підписки")
        };

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            CustomerEmail = userEmail,
            Metadata = new Dictionary<string, string>
            {
                { "tier", tier.ToLower() }
            },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = priceId,
                    Quantity = 1
                }
            },
            Mode = "subscription",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }
}
