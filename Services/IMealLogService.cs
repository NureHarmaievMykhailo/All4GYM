using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IMealLogService
{
    Task<List<MealLogDto>> GetAllAsync(int userId);
    Task<MealLogDto> GetByIdAsync(int id, int userId);
    Task<MealLogDto> CreateAsync(CreateMealLogDto dto, int userId);
    Task<MealLogDto> UpdateAsync(int id, CreateMealLogDto dto, int userId);
    Task DeleteAsync(int id, int userId);
}