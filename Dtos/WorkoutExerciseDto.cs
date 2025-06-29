namespace All4GYM.Dtos;

public class WorkoutExerciseDto
{
    public int WorkoutId { get; set; }
    public int ExerciseId { get; set; }

    public string ExerciseName { get; set; } = null!;
    public int Sets { get; set; }
    public int Reps { get; set; }
    public float Weight { get; set; }
}