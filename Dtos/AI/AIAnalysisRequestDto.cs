using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace All4GYM.Dtos.AI;

public class AIAnalysisRequestDto
{
    [Required]
    [JsonPropertyName("periodDays")]
    public int PeriodDays { get; set; }

    [Required]
    [JsonPropertyName("vectorType")]
    public string VectorType { get; set; } = null!;
}