using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterUserDto dto);
    Task<string> LoginAsync(LoginDto dto);
    Task<UserDto> GetByIdAsync(int id);
    Task<UserDto> UpdateAsync(int id, RegisterUserDto dto);
    Task DeleteAsync(int id);
    Task<List<UserDto>> GetAllAsync();
}