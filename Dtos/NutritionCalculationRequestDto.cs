using System.Text.Json.Serialization;
using All4GYM.Dtos.Enums; 

namespace All4GYM.Dtos;

public class NutritionCalculationRequestDto
{
    [JsonPropertyName("gender")]
    public GenderType Gender { get; set; }

    [JsonPropertyName("age")]
    public int Age { get; set; }
    
    [JsonPropertyName("heightCm")]
    public double HeightCm { get; set; }

    [JsonPropertyName("weightKg")]
    public double WeightKg { get; set; }

    [JsonPropertyName("activityLevel")]
    public ActivityLevel Activity { get; set; }
    
    [JsonPropertyName("goal")]
    public FitnessGoal Goal { get; set; }
}

