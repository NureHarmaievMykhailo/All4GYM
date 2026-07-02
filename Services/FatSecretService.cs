using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using All4GYM.Attributes;
using All4GYM.Dtos;
using Microsoft.Extensions.Options;

namespace All4GYM.Services;

public class FatSecretService : IFatSecretService
{
    private readonly HttpClient _httpClient;
    private readonly FatSecretOptions _options;
    
    private string? _accessToken;
    private DateTime _tokenExpiration = DateTime.MinValue;

    public FatSecretService(HttpClient httpClient, IOptions<FatSecretOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }
    
    private async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiration)
        {
            return _accessToken;
        }

        var request = new HttpRequestMessage(HttpMethod.Post, _options.CredentialsUrl);
        var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);
        
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "scope", "basic" }
        });

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        _accessToken = root.GetProperty("access_token").GetString();
        int expiresIn = root.GetProperty("expires_in").GetInt32();
        _tokenExpiration = DateTime.UtcNow.AddSeconds(expiresIn - 60); // Запас в 1 минуту

        return _accessToken!;
    }
    
    public async Task<List<FatSecretProductDto>> SearchFoodItemsAsync(string query, int pageNumber = 0, int maxResults = 15)
    {
        try
        {
            var token = await GetAccessTokenAsync();
            var requestUrl = $"{_options.ApiBaseUrl}?method=foods.search&search_expression={Uri.EscapeDataString(query)}&page_number={pageNumber}&max_results={maxResults}&format=json";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<FatSecretProductDto>();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            
            if (!doc.RootElement.TryGetProperty("foods", out var foodsProp) || 
                !foodsProp.TryGetProperty("food", out var foodListProp))
            {
                return new List<FatSecretProductDto>();
            }

            var result = new List<FatSecretProductDto>();
            
            if (foodListProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in foodListProp.EnumerateArray())
                {
                    result.Add(MapJsonToProductDto(item));
                }
            }
            else if (foodListProp.ValueKind == JsonValueKind.Object)
            {
                result.Add(MapJsonToProductDto(foodListProp));
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FatSecret Search Error]: {ex.Message}");
            return new List<FatSecretProductDto>();
        }
    }
    
    public async Task<FatSecretProductDetailsDto> GetFoodItemDetailsAsync(long foodId)
    {
        var token = await GetAccessTokenAsync();
        var requestUrl = $"{_options.ApiBaseUrl}?method=food.get.v2&food_id={foodId}&format=json";

        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var food = doc.RootElement.GetProperty("food");
        var servings = food.GetProperty("servings").GetProperty("serving");
        
        JsonElement serving = servings.ValueKind == JsonValueKind.Array ? servings[0] : servings;
        
        if (servings.ValueKind == JsonValueKind.Array)
        {
            foreach (var s in servings.EnumerateArray())
            {
                var metricUnit = s.TryGetProperty("metric_serving_unit", out var u) ? u.GetString() : "";
                var metricAmount = s.TryGetProperty("metric_serving_amount", out var a) ? a.GetString() : "";
                if ((metricUnit == "g" || metricUnit == "ml") && metricAmount == "100.000")
                {
                    serving = s;
                    break;
                }
            }
        }

        float metricAmountValue = serving.TryGetProperty("metric_serving_amount", out var am) ? float.Parse(am.GetString() ?? "100") : 100f;
        float multiplier = 100f / (metricAmountValue <= 0 ? 100f : metricAmountValue);

        return new FatSecretProductDetailsDto
        {
            FoodId = foodId,
            Name = food.GetProperty("food_name").GetString()!,
            Calories = float.Parse(serving.GetProperty("calories").GetString()!) * multiplier,
            Proteins = float.Parse(serving.GetProperty("protein").GetString()!) * multiplier,
            Fats = float.Parse(serving.GetProperty("fat").GetString()!) * multiplier,
            Carbs = float.Parse(serving.GetProperty("carbohydrate").GetString()!) * multiplier
        };
    }

    private FatSecretProductDto MapJsonToProductDto(JsonElement item)
    {
        return new FatSecretProductDto
        {
            FoodId = long.Parse(item.GetProperty("food_id").GetString()!),
            FoodName = item.GetProperty("food_name").GetString()!,
            BrandName = item.TryGetProperty("brand_name", out var b) ? b.GetString() ?? "" : "",
            FoodDescription = item.GetProperty("food_description").GetString()!
        };
    }
}