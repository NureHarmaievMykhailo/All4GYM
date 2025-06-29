using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IProgressLogService
{
    Task<List<ProgressLogDto>> GetAllAsync(int userId);
    Task<ProgressLogDto> GetByIdAsync(int id, int userId);
    Task<ProgressLogDto> CreateAsync(CreateProgressLogDto dto, int userId);
    Task<ProgressLogDto> UpdateAsync(int id, CreateProgressLogDto dto, int userId);
    Task DeleteAsync(int id, int userId);
}