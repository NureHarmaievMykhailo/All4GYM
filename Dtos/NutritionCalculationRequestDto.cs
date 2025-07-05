using System.Text.Json.Serialization;

namespace All4GYM.Dtos;

public class NutritionCalculationRequestDto
{
    [JsonPropertyName("gender")]
    public string Gender { get; set; } = "";

    [JsonPropertyName("age")]
    public int Age { get; set; }
    
    [JsonPropertyName("height")]
    public float HeightCm { get; set; }

    [JsonPropertyName("weight")]
    public float WeightKg { get; set; }

    [JsonPropertyName("activityLevel")]
    public string ActivityLevel { get; set; } = "";

    [JsonPropertyName("goal")]
    public string Goal { get; set; } = "";
}

