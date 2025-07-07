using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IPaymentService
{
    Task<string> CreateSubscriptionCheckoutSessionAsync(CreateSubscriptionPaymentDto dto, string userEmail);
}
