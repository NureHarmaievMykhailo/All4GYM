namespace All4GYM.Dtos;

public class UpdateUserProfileDto
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Password { get; set; }

    public int? Age { get; set; }
    public double? HeightCm { get; set; }
    public double? WeightKg { get; set; }
    public string? Gender { get; set; }
    public string? Goal { get; set; }
    public string? PreferredWorkoutDays { get; set; }
    public string? GymPassCode { get; set; }
}
