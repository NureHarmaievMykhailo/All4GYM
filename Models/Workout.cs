namespace All4GYM.Models;

public class Workout
{
    public int Id { get; set; }
    public int TrainingProgramId { get; set; }
    public TrainingProgram TrainingProgram { get; set; } = null!;

    public DateTime Date { get; set; }
    public string? Notes { get; set; }

    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
}