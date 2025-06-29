using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace All4GYM.Controllers;

[ApiController]
[Authorize]
[Route("api/workouts/{workoutId}/exercises")]
public class WorkoutExerciseController : ControllerBase
{
    private readonly IWorkoutExerciseService _service;

    public WorkoutExerciseController(IWorkoutExerciseService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll(int workoutId)
    {
        var result = await _service.GetByWorkoutIdAsync(workoutId, GetUserId());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int workoutId, [FromBody] AddWorkoutExerciseDto dto)
    {
        await _service.AddAsync(workoutId, dto, GetUserId());
        return Ok(new { message = "Вправу додано до тренування" });
    }

    [HttpDelete("{exerciseId}")]
    public async Task<IActionResult> Remove(int workoutId, int exerciseId)
    {
        await _service.RemoveAsync(workoutId, exerciseId, GetUserId());
        return NoContent();
    }
}