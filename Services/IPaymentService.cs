using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IPaymentService
{
    Task<PaymentDto> CreateAsync(CreatePaymentDto dto, int userId);
}