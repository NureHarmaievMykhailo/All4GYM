using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IVideoContentService
{
    Task<List<VideoContentDto>> GetAllAsync();
    Task<VideoContentDto> GetByIdAsync(int id);
    Task<VideoContentDto> CreateAsync(CreateVideoContentDto dto);
    Task<VideoContentDto> UpdateAsync(int id, CreateVideoContentDto dto);
    Task DeleteAsync(int id);
}