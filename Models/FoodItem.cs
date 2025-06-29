namespace All4GYM.Models;

public class FoodItem
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public int Calories { get; set; }
    public float Proteins { get; set; }
    public float Fats { get; set; }
    public float Carbs { get; set; }
}