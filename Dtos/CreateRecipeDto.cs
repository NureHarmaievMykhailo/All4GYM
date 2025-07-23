using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace All4GYM.Dtos;

public class CreateRecipeDto
{
    [Required(ErrorMessage = "Вкажіть назву рецепта")]
    [MaxLength(150)]
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Додайте опис або інструкцію")]
    [MaxLength(2000)]
    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    [Range(0, 1200, ErrorMessage = "Калорійність 0‑1200 ккал")]
    [JsonPropertyName("calories")]
    public int Calories { get; set; }

    [Range(0, 200, ErrorMessage = "Білок 0‑200 г")]
    [JsonPropertyName("proteins")]
    public float Proteins { get; set; }

    [Range(0, 200, ErrorMessage = "Жири 0‑200 г")]
    [JsonPropertyName("fats")]
    public float Fats { get; set; }

    [Range(0, 300, ErrorMessage = "Вуглеводи 0‑300 г")]
    [JsonPropertyName("carbs")]
    public float Carbs { get; set; }

    [Required(ErrorMessage = "Вкажіть коректний URL картинки")]
    [MaxLength(500)]
    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }
}