using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IFoodItemService
{
    Task<List<FoodItemDto>> GetAllAsync();
    Task<FoodItemDto> GetByIdAsync(int id);
    Task<FoodItemDto> CreateAsync(CreateFoodItemDto dto);
    Task<FoodItemDto> UpdateAsync(int id, CreateFoodItemDto dto);
    Task DeleteAsync(int id);
}