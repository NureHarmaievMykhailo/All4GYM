using System.Text.Json.Serialization;
using All4GYM.Models;

namespace All4GYM.Dtos;

public class MealLogDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [JsonPropertyName("foodItemId")]
    public int FoodItemId { get; set; }
    
    [JsonPropertyName("foodItemName")]
    public string? FoodItemName { get; set; }
    
    public FoodItem FoodItem { get; set; } = null!;

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("grams")]
    public float Grams { get; set; }

    [JsonPropertyName("calories")]
    public int Calories { get; set; }
    
    [JsonPropertyName("proteins")]
    public float Proteins { get; set; }
    
    [JsonPropertyName("fats")]
    public float Fats { get; set; }
    
    [JsonPropertyName("carbs")]
    public float Carbs { get; set; }

    [JsonPropertyName("mealType")]
    public string MealType { get; set; } = null!;

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}