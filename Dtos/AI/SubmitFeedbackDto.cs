using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace All4GYM.Dtos.AI;

public class SubmitFeedbackDto
{
    [Required]
    [JsonPropertyName("aiReviewId")]
    public int AIReviewId { get; set; }

    [Required]
    [JsonPropertyName("isHelpful")]
    public bool IsHelpful { get; set; }

    [JsonPropertyName("userComments")]
    public string? UserComments { get; set; }
}