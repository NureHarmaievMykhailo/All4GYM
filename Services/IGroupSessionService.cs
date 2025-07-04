using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IGroupSessionService
{
    Task<List<GroupSessionDto>> GetAllAsync();
    Task<GroupSessionDto> GetByIdAsync(int id);
    Task<GroupSessionDto> CreateAsync(CreateGroupSessionDto dto);
    Task<GroupSessionDto> UpdateAsync(int id, CreateGroupSessionDto dto);
    Task DeleteAsync(int id);
}