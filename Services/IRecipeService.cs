using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IRecipeService
{
    Task<List<RecipeDto>> GetAllAsync();
    Task<RecipeDto> CreateAsync(CreateRecipeDto dto);
    // (опціонально) Update / Delete
}