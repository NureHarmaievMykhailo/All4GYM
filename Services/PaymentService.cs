using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using All4GYM.Services.Stripe;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;
    private readonly StripePaymentIntentService _stripe;

    public PaymentService(AppDbContext context, StripePaymentIntentService stripe)
    {
        _context = context;
        _stripe = stripe;
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto, int userId)
    {
        var order = await _context.Orders
                        .FirstOrDefaultAsync(o => o.Id == dto.OrderId && o.UserId == userId)
                    ?? throw new Exception("Замовлення не знайдено або не належить вам");

        var amountInCents = (long)(order.TotalAmount * 100);

        var intent = await _stripe.CreateAsync(amountInCents, "usd", order.Id.ToString());

        var payment = new Payment
        {
            UserId = userId,
            OrderId = order.Id,
            Amount = order.TotalAmount,
            StripePaymentId = intent.Id,
            Status = "Pending",
            Date = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return new PaymentDto
        {
            ClientSecret = intent.ClientSecret
        };
    }

    public async Task<string> CreateSubscriptionCheckoutSessionAsync(CreateSubscriptionPaymentDto dto, string userEmail)
    {
        var successUrl = "http://localhost:5263/SubscriptionSuccess";
        var cancelUrl = "http://localhost:5263/SubscriptionCancel";

        return await _stripe.CreateCheckoutSessionAsync(userEmail, dto.Tier, successUrl, cancelUrl);
    }
}