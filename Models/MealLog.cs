namespace All4GYM.Models;

public class MealLog
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime Date { get; set; }
    public int Calories { get; set; }
    public float Proteins { get; set; }
    public float Fats { get; set; }
    public float Carbs { get; set; }

    public string? Notes { get; set; }
}