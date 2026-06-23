namespace All4GYM.Models;

public class UserFeedback
{
    public int Id { get; set; }
    
    public int AIReviewId { get; set; }
    public AIReview AIReview { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public bool IsHelpful { get; set; } // true = користно, false = ні
    public string? UserComments { get; set; }
    public DateTime CreatedAt { get; set; }
}