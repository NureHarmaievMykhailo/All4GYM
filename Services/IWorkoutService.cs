using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IWorkoutService
{
    Task<List<WorkoutDto>> GetAllAsync(int userId);
    Task<WorkoutDto> GetByIdAsync(int id, int userId);
    Task<WorkoutDto> CreateAsync(CreateWorkoutDto dto, int userId);
    Task<WorkoutDto> UpdateAsync(int id, CreateWorkoutDto dto, int userId);
    Task DeleteAsync(int id, int userId);
}