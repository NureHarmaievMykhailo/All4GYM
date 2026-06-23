using System.ComponentModel.DataAnnotations;

namespace All4GYM.Dtos.AI;

public class AIAnalysisRequestDto
{
    [Required]
    public int PeriodDays { get; set; }

    [Required]
    public string VectorType { get; set; } = null!;
}