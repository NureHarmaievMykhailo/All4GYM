namespace All4GYM.Dtos;

public class CreateExerciseDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? MuscleGroup { get; set; }
}