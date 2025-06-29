using All4GYM.Dtos;

namespace All4GYM.Services;

public interface ITrainingProgramService
{
    Task<List<TrainingProgramDto>> GetAllAsync(int userId);
    Task<TrainingProgramDto> GetByIdAsync(int id, int userId);
    Task<TrainingProgramDto> CreateAsync(CreateTrainingProgramDto dto, int userId);
    Task<TrainingProgramDto> UpdateAsync(int id, CreateTrainingProgramDto dto, int userId);
    Task DeleteAsync(int id, int userId);
}