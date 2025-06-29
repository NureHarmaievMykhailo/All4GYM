using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class MealLogService : IMealLogService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public MealLogService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<MealLogDto>> GetAllAsync(int userId)
    {
        var logs = await _context.MealLogs
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.Date)
            .ToListAsync();

        return _mapper.Map<List<MealLogDto>>(logs);
    }

    public async Task<MealLogDto> GetByIdAsync(int id, int userId)
    {
        var log = await _context.MealLogs
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId)
            ?? throw new Exception("Запис не знайдено");

        return _mapper.Map<MealLogDto>(log);
    }

    public async Task<MealLogDto> CreateAsync(CreateMealLogDto dto, int userId)
    {
        var log = _mapper.Map<MealLog>(dto);
        log.UserId = userId;

        _context.MealLogs.Add(log);
        await _context.SaveChangesAsync();

        return _mapper.Map<MealLogDto>(log);
    }

    public async Task<MealLogDto> UpdateAsync(int id, CreateMealLogDto dto, int userId)
    {
        var log = await _context.MealLogs
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId)
            ?? throw new Exception("Запис не знайдено");

        log.Date = dto.Date;
        log.Calories = dto.Calories;
        log.Proteins = dto.Proteins;
        log.Fats = dto.Fats;
        log.Carbs = dto.Carbs;
        log.Notes = dto.Notes;

        await _context.SaveChangesAsync();
        return _mapper.Map<MealLogDto>(log);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var log = await _context.MealLogs
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId)
            ?? throw new Exception("Запис не знайдено");

        _context.MealLogs.Remove(log);
        await _context.SaveChangesAsync();
    }
}
