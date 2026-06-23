namespace All4GYM.Models;

public class AIReview
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string VectorType { get; set; } = null!;
    public DateTime GeneratedAt { get; set; }
    public int PeriodDays { get; set; }

    public string Overview { get; set; } = null!;
    
    public string RecommendationsJson { get; set; } = null!; 
    
    public string TrendPrediction { get; set; } = null!;
}