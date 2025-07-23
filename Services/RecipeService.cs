using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class RecipeService : IRecipeService
{
    private readonly AppDbContext _context;
    private readonly IMapper _map;

    public RecipeService(AppDbContext context, IMapper map)
    {
        _context = context;
        _map = map;
    }

    public async Task<List<RecipeDto>> GetAllAsync()
        => _map.Map<List<RecipeDto>>(await _context.Recipes.ToListAsync());

    public async Task<RecipeDto> CreateAsync(CreateRecipeDto dto)
    {
        var entity = _map.Map<Recipe>(dto);
        _context.Recipes.Add(entity);
        await _context.SaveChangesAsync();
        return _map.Map<RecipeDto>(entity);
    }
}