using Microsoft.Extensions.Configuration;
using Stripe;

namespace All4GYM.Services.Stripe;

public class StripePaymentIntentService
{
    private readonly PaymentIntentService _paymentIntentService;

    public StripePaymentIntentService()
    {
        _paymentIntentService = new PaymentIntentService();
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
            }
        };

        return await _paymentIntentService.CreateAsync(options);
    }
}