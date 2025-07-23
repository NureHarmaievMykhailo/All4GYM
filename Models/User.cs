namespace All4GYM.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public bool HasActiveSubscription { get; set; } = false;
    public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Basic;
    
    public int? Age { get; set; }
    public double? HeightCm { get; set; }
    public double? WeightKg { get; set; }
    public string? Gender { get; set; }
    public string? Goal { get; set; }
    public string? PreferredWorkoutDays { get; set; }
    public string? GymPassCode { get; set; }

    public ICollection<TrainingProgram> TrainingPrograms { get; set; } = new List<TrainingProgram>();
    public ICollection<MealLog> MealLogs { get; set; } = new List<MealLog>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<ProgressLog> ProgressLogs { get; set; } = new List<ProgressLog>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}