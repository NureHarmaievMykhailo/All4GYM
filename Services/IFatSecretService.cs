using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IFatSecretService
{
    Task<List<FatSecretProductDto>> SearchFoodItemsAsync(string query, int pageNumber = 0, int maxResults = 15);
    Task<FatSecretProductDetailsDto> GetFoodItemDetailsAsync(long foodId);
}