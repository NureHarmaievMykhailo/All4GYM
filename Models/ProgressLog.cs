namespace All4GYM.Models;

public class ProgressLog
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime Date { get; set; }
    public float Weight { get; set; }
    public float MuscleMass { get; set; }
    public float BodyFat { get; set; }
    public string? Notes { get; set; }
}