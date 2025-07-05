using System.Text.Json.Serialization;

namespace All4GYM.Dtos;

public class FoodItemDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("calories")]
    public int Calories { get; set; }

    [JsonPropertyName("proteins")]
    public float Proteins { get; set; }

    [JsonPropertyName("fats")]
    public float Fats { get; set; }

    [JsonPropertyName("carbs")]
    public float Carbs { get; set; }
}