using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllAsync(int userId);
    Task<OrderDto> GetByIdAsync(int id, int userId);
    Task<OrderDto> CreateAsync(CreateOrderDto dto, int userId);
}