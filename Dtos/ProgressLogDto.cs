namespace All4GYM.Dtos;

public class ProgressLogDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public float Weight { get; set; }
    public float MuscleMass { get; set; }
    public float BodyFat { get; set; }
    public string? Notes { get; set; }
}