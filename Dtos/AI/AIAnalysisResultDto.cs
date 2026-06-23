using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace All4GYM.Dtos.AI;

public class AIAnalysisResultDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("vectorType")]
    public string VectorType { get; set; } = null!;
    [JsonPropertyName("generatedAt")]
    public DateTime GeneratedAt { get; set; }
    [JsonPropertyName("periodDays")]
    public int PeriodDays { get; set; }
    [JsonPropertyName("overview")]
    public string Overview { get; set; } = null!;
    [JsonPropertyName("recommendations")]
    public List<string> Recommendations { get; set; } = new();
    [JsonPropertyName("trendPrediction")]
    public string TrendPrediction { get; set; } = null!;
}