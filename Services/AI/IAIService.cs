using All4GYM.Dtos.AI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace All4GYM.Services.AI;

public interface IAIService
{
    Task<AIAnalysisResultDto> GenerateReviewAsync(int userId, AIAnalysisRequestDto dto);
    Task<bool> SubmitFeedbackAsync(int userId, SubmitFeedbackDto dto);
    Task<List<AIAnalysisResultDto>> GetUserReviewHistoryAsync(int userId, string vectorType);
    Task<WorkoutOptimizationResultDto> OptimizeWorkoutAsync(int userId, int currentWorkoutId);
    Task<DailyNutritionAnalysisDto> AnalyzeDailyNutritionAsync(int userId, DateTime targetDate);
}