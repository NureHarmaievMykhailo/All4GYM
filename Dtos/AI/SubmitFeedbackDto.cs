using System.ComponentModel.DataAnnotations;

namespace All4GYM.Dtos.AI;

public class SubmitFeedbackDto
{
    [Required]
    public int AIReviewId { get; set; }

    [Required]
    public bool IsHelpful { get; set; }

    public string? UserComments { get; set; }
}