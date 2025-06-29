using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IExerciseService
{
    Task<List<ExerciseDto>> GetAllAsync();
    Task<ExerciseDto> GetByIdAsync(int id);
    Task<ExerciseDto> CreateAsync(CreateExerciseDto dto);
    Task<ExerciseDto> UpdateAsync(int id, CreateExerciseDto dto);
    Task DeleteAsync(int id);
}