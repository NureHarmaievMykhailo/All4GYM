using System;
using System.Collections.Generic;

namespace All4GYM.Dtos.AI;

public class AIAnalysisResultDto
{
    public int Id { get; set; }
    public string VectorType { get; set; } = null!;
    public DateTime GeneratedAt { get; set; }
    public int PeriodDays { get; set; }
    public string Overview { get; set; } = null!;
    public List<string> Recommendations { get; set; } = new();
    public string TrendPrediction { get; set; } = null!;
}