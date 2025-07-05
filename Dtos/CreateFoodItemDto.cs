using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace All4GYM.Dtos;

public class CreateFoodItemDto
{
    [Required(ErrorMessage = "Назва обов’язкова")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [Range(0, 8000, ErrorMessage = "Калорійність має бути від 0 до 8000")]
    [JsonPropertyName("calories")]
    public int Calories { get; set; }

    [Range(0, 300, ErrorMessage = "Білків не може бути більше 300г")]
    [JsonPropertyName("proteins")]
    public float Proteins { get; set; }

    [Range(0, 300, ErrorMessage = "Жирів не може бути більше 300г")]
    [JsonPropertyName("fats")]
    public float Fats { get; set; }

    [Range(0, 500, ErrorMessage = "Вуглеводів не може бути більше 500г")]
    [JsonPropertyName("carbs")]
    public float Carbs { get; set; }
}