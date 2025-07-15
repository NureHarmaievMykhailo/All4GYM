using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace All4GYM.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly JwtService _jwtService;

    public UserService(AppDbContext context, IMapper mapper, JwtService jwtService)
    {
        _context = context;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    public async Task<UserDto> RegisterAsync(RegisterUserDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new Exception("Користувач з таким email вже існує");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId = dto.RoleId ?? 1 // за замовчуванням — Client
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Невірний логін або пароль");

        return _jwtService.GenerateToken(user);
    }
    
    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id)
                   ?? throw new Exception("Користувача не знайдено");
        return _mapper.Map<UserDto>(user);
    }
    
    public async Task<UserDto> UpdateAsync(int id, RegisterUserDto dto)
    {
        var user = await _context.Users.FindAsync(id) 
                   ?? throw new Exception("Користувача не знайдено");

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _context.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id)
                   ?? throw new Exception("Користувача не знайдено");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _context.Users.Include(u => u.Role).ToListAsync();
        return _mapper.Map<List<UserDto>>(users);
    }
    
    public async Task<UserDto> GetUserWithSubscriptionAsync(int userId)
    {
        var user = await _context.Users
                       .Include(u => u.Role)
                       .FirstOrDefaultAsync(u => u.Id == userId)
                   ?? throw new Exception("Користувача не знайдено");

        var dto = _mapper.Map<UserDto>(user);

        var activeSub = await _context.Subscriptions
            .Where(s => s.UserId == userId && s.IsActive && s.EndDate > DateTime.UtcNow)
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefaultAsync();

        dto.HasActiveSubscription = activeSub != null;

        if (Enum.TryParse<SubscriptionTier>(activeSub?.Type, out var parsedTier))
            dto.SubscriptionTier = parsedTier;
        else
            dto.SubscriptionTier = SubscriptionTier.Basic;

        return dto;
    }
    
}