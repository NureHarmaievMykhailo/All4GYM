using All4GYM.Dtos;
using All4GYM.Models;

namespace All4GYM.Services;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterUserDto dto);
    Task<string> LoginAsync(LoginDto dto);
    Task<UserDto> GetByIdAsync(int id);
    Task<UserDto> UpdateAsync(int id, UpdateUserProfileDto dto);
    Task DeleteAsync(int id);
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> GetUserWithSubscriptionAsync(int userId);
}