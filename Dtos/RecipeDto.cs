using System.Text.Json.Serialization;

namespace All4GYM.Dtos;

public class RecipeDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    [JsonPropertyName("calories")]
    public int Calories { get; set; }

    [JsonPropertyName("proteins")]
    public float Proteins { get; set; }

    [JsonPropertyName("fats")]
    public float Fats { get; set; }

    [JsonPropertyName("carbs")]
    public float Carbs { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; } = null!;
}