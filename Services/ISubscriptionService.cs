using All4GYM.Dtos;

namespace All4GYM.Services;

public interface ISubscriptionService
{
    Task<List<SubscriptionDto>> GetAllAsync(int userId);
    Task<SubscriptionDto> GetByIdAsync(int id, int userId);
    Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto, int userId);
    Task<SubscriptionDto> UpdateAsync(int id, CreateSubscriptionDto dto, int userId);
    Task DeleteAsync(int id, int userId);
    Task CancelAsync(int userId);

}