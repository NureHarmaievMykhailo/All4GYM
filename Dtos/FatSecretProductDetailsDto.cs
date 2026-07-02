namespace All4GYM.Dtos;

public class FatSecretProductDetailsDto
{
    public long FoodId { get; set; }
    public string Name { get; set; } = null!;
    public float Calories { get; set; }
    public float Proteins { get; set; }
    public float Fats { get; set; }
    public float Carbs { get; set; }
}