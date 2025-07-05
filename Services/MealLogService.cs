using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class MealLogService : IMealLogService
{
    private readonly AppDbContext _context;

    public MealLogService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<MealLogDto>> GetMealLogsAsync(int userId, DateTime? date = null)
    {
        var query = _context.MealLogs
            .Include(m => m.FoodItem)
            .Where(m => m.UserId == userId);

        if (date.HasValue)
        {
            var day = date.Value.Date;
            query = query.Where(m => m.Date.Date == day);
        }

        var logs = await query
            .OrderByDescending(m => m.Date)
            .ToListAsync();

        return logs.Select(log => new MealLogDto
        {
            Id = log.Id,
            Date = log.Date,
            Calories = log.Calories,
            Proteins = log.Proteins,
            Fats = log.Fats,
            Carbs = log.Carbs,
            Notes = log.Notes,
            MealType = log.MealType.ToString(),
            FoodItemName = log.FoodItem.Name,
            Grams = log.Grams
        }).ToList();
    }

    public async Task<MealLogDto> GetByIdAsync(int id, int userId)
    {
        var log = await _context.MealLogs
            .Include(l => l.FoodItem)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId)
            ?? throw new Exception("Запис не знайдено");

        return new MealLogDto
        {
            Id = log.Id,
            Date = log.Date,
            Calories = log.Calories,
            Proteins = log.Proteins,
            Fats = log.Fats,
            Carbs = log.Carbs,
            Notes = log.Notes,
            FoodItemId = log.FoodItemId,
            FoodItemName = log.FoodItem?.Name,
            Grams = log.Grams,
            MealType = log.MealType.ToString()
        };
    }

    public async Task<MealLogDto> CreateAsync(CreateMealLogDto dto, int userId)
    {
        var food = await _context.FoodItems.FindAsync(dto.FoodItemId)
            ?? throw new Exception("Продукт не знайдено");

        var multiplier = dto.Grams / 100f;

        var log = new MealLog
        {
            UserId = userId,
            Date = dto.Date,
            FoodItemId = food.Id,
            Grams = dto.Grams,
            Calories = (int)(food.Calories * multiplier),
            Proteins = food.Proteins * multiplier,
            Fats = food.Fats * multiplier,
            Carbs = food.Carbs * multiplier,
            MealType = dto.MealType,
            Notes = dto.Notes
        };

        _context.MealLogs.Add(log);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(log.Id, userId);
    }

    public async Task<MealLogDto> UpdateAsync(int id, CreateMealLogDto dto, int userId)
    {
        var log = await _context.MealLogs.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId)
            ?? throw new Exception("Запис не знайдено");

        var food = await _context.FoodItems.FindAsync(dto.FoodItemId)
            ?? throw new Exception("Продукт не знайдено");

        var multiplier = dto.Grams / 100f;

        log.Date = dto.Date;
        log.FoodItemId = food.Id;
        log.Grams = dto.Grams;
        log.Calories = (int)(food.Calories * multiplier);
        log.Proteins = food.Proteins * multiplier;
        log.Fats = food.Fats * multiplier;
        log.Carbs = food.Carbs * multiplier;
        log.MealType = dto.MealType;
        log.Notes = dto.Notes;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(log.Id, userId);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var log = await _context.MealLogs.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId)
            ?? throw new Exception("Запис не знайдено");

        _context.MealLogs.Remove(log);
        await _context.SaveChangesAsync();
    }
}
