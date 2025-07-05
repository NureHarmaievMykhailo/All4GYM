using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using All4GYM.Models;

namespace All4GYM.Dtos;

public class CreateMealLogDto
{
    [Required]
    [JsonPropertyName("foodItemId")]
    public int FoodItemId { get; set; }

    [Required]
    [Range(1, 1000, ErrorMessage = "Введіть кількість грамів від 1 до 1000")]
    [JsonPropertyName("grams")]
    public float Grams { get; set; }

    [Required]
    [JsonPropertyName("mealType")]
    public MealType MealType { get; set; }

    [Required]
    [JsonPropertyName("date")]
    public DateTime Date { get; set; } = DateTime.Today;

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
