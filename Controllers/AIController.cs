using All4GYM.Dtos.AI;
using All4GYM.Services.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace All4GYM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class AIController : ControllerBase
{
    private readonly IAIService _aiService;

    public AIController(IAIService aiService)
    {
        _aiService = aiService;
    }
    
    [HttpPost("review")]
    public async Task<IActionResult> GetAIReview([FromBody] AIAnalysisRequestDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await _aiService.GenerateReviewAsync(userId, dto);
        return Ok(result);
    }

    [HttpPost("feedback")]
    public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var success = await _aiService.SubmitFeedbackAsync(userId, dto);
        if (!success) return BadRequest("Could not submit feedback.");
        return Ok(new { message = "Feedback submitted successfully" });
    }

    [HttpGet("history/{vectorType}")]
    public async Task<IActionResult> GetHistory(string vectorType)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var history = await _aiService.GetUserReviewHistoryAsync(userId, vectorType);
        return Ok(history);
    }
    
    [HttpGet("optimize-workout/{currentWorkoutId}")]
    public async Task<IActionResult> OptimizeWorkout(int currentWorkoutId)
    {
        try
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            var userId = int.Parse(userIdStr);
            var result = await _aiService.OptimizeWorkoutAsync(userId, currentWorkoutId);
            
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}