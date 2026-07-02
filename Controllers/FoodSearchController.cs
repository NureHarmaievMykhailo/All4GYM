using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodSearchController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IFatSecretService _fatSecretService;

    public FoodSearchController(AppDbContext context, IFatSecretService fatSecretService)
    {
        _context = context;
        _fatSecretService = fatSecretService;
    }

    [HttpGet("autocomplete")]
    public async Task<ActionResult<List<FoodSearchResultDto>>> Autocomplete([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return Ok(new List<FoodSearchResultDto>());
        }
        
        var localResults = await _context.FoodItems
            .Where(f => f.Name.Contains(query))
            .Take(5)
            .Select(f => new FoodSearchResultDto
            {
                Id = $"local_{f.Id}",
                Name = f.Name,
                Description = $"{f.Calories} ккал | Б:{f.Proteins}г Ж:{f.Fats}г В:{f.Carbs}г",
                Source = "Local"
            })
            .ToListAsync();
        
        var fatSecretResults = await _fatSecretService.SearchFoodItemsAsync(query, pageNumber: 0, maxResults: 10);
        
        var externalResults = fatSecretResults.Select(fs => new FoodSearchResultDto
        {
            Id = $"fatsecret_{fs.FoodId}",
            Name = string.IsNullOrEmpty(fs.BrandName) ? fs.FoodName : $"{fs.FoodName} ({fs.BrandName})",
            Description = CleanFatSecretDescription(fs.FoodDescription),
            Source = "FatSecret"
        });
        
        var finalSearchList = localResults.Concat(externalResults).ToList();

        return Ok(finalSearchList);
    }
    
    private string CleanFatSecretDescription(string desc)
    {
        if (string.IsNullOrEmpty(desc)) return desc;

        var parts = desc.Split('|');
        if (parts.Length < 4) return desc;

        string cals = parts[0].Substring(parts[0].IndexOf("Calories:") + 9).Replace("kcal", " ккал").Trim();
        string fat = parts[1].Replace("Fat:", "Ж:").Replace("g", "г").Trim();
        string carbs = parts[2].Replace("Carbs:", "В:").Replace("g", "г").Trim();
        string prot = parts[3].Replace("Protein:", "Б:").Replace("g", "г").Trim();

        return $"{cals} | {prot} {fat} {carbs}";
    }
}