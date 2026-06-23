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
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _aiService.GenerateReviewAsync(userId, dto);
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, $"AI Service error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal error: {ex.Message}");
        }
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
}