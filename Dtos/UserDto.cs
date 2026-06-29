using All4GYM.Models;

namespace All4GYM.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public bool HasActiveSubscription { get; set; } = false;
    public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Basic;
    
    public int? Age { get; set; }
    public double? HeightCm { get; set; }
    public double? WeightKg { get; set; }
    public string? Gender { get; set; }
    public string? Goal { get; set; }
    public string? PreferredWorkoutDays { get; set; }
    public string? GymPassCode { get; set; }
    
    public int TargetCalories { get; set; }
    public float TargetProteins { get; set; }
    public float TargetFats { get; set; }
    public float TargetCarbs { get; set; }
}