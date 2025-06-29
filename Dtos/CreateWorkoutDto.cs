namespace All4GYM.Dtos;

public class CreateWorkoutDto
{
    public int TrainingProgramId { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
}