using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class FoodItemService : IFoodItemService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public FoodItemService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<FoodItemDto>> GetAllAsync()
    {
        var items = await _context.FoodItems.ToListAsync();
        return _mapper.Map<List<FoodItemDto>>(items);
    }

    public async Task<FoodItemDto> GetByIdAsync(int id)
    {
        var item = await _context.FoodItems.FindAsync(id)
                   ?? throw new Exception("Продукт не знайдено");

        return _mapper.Map<FoodItemDto>(item);
    }

    public async Task<FoodItemDto> CreateAsync(CreateFoodItemDto dto)
    {
        var entity = _mapper.Map<FoodItem>(dto);
        _context.FoodItems.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<FoodItemDto>(entity);
    }

    public async Task<FoodItemDto> UpdateAsync(int id, CreateFoodItemDto dto)
    {
        var item = await _context.FoodItems.FindAsync(id)
                   ?? throw new Exception("Продукт не знайдено");

        item.Name = dto.Name;
        item.Calories = dto.Calories;
        item.Proteins = dto.Proteins;
        item.Fats = dto.Fats;
        item.Carbs = dto.Carbs;

        await _context.SaveChangesAsync();
        return _mapper.Map<FoodItemDto>(item);
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _context.FoodItems.FindAsync(id)
                   ?? throw new Exception("Продукт не знайдено");

        _context.FoodItems.Remove(item);
        await _context.SaveChangesAsync();
    }
}