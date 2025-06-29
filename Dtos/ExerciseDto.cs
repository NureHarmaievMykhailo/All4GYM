namespace All4GYM.Dtos;

public class ExerciseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? MuscleGroup { get; set; }
}