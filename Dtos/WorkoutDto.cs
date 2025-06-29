namespace All4GYM.Dtos;

public class WorkoutDto
{
    public int Id { get; set; }
    public int TrainingProgramId { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
}