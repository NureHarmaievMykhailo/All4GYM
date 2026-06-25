using All4GYM.Dtos;

namespace All4GYM.Services;

public interface ITrainingProgramService
{
    Task<List<TrainingProgramDto>> GetAllAsync();
    Task<TrainingProgramDto> GetByIdAsync(int id);
    Task<TrainingProgramDto> CreateAsync(CreateTrainingProgramDto dto);
    Task<TrainingProgramDto> UpdateAsync(int id, CreateTrainingProgramDto dto);
    Task DeleteAsync(int id);
}