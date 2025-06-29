namespace All4GYM.Dtos;

public class MealLogDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int Calories { get; set; }
    public float Proteins { get; set; }
    public float Fats { get; set; }
    public float Carbs { get; set; }
    public string? Notes { get; set; }
}