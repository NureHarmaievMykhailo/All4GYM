namespace All4GYM.Models;

public class TrainingProgram
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public string? Category { get; set; }

    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}