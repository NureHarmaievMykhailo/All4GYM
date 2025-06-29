namespace All4GYM.Models;

public class WorkoutExercise
{
    public int WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public int Sets { get; set; }
    public int Reps { get; set; }
    public float Weight { get; set; }
}