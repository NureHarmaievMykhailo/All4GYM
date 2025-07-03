using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace All4GYM.Controllers;

[ApiController]
[Authorize(Roles = "User,Coach,Admin")]
[Route("api/workouts/{workoutId}/exercises")]
public class WorkoutExerciseController : ControllerBase
{
    private readonly IWorkoutExerciseService _service;

    public WorkoutExerciseController(IWorkoutExerciseService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Отримати вправи у тренуванні.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Отримати вправи тренування")]
    public async Task<IActionResult> GetAll(int workoutId)
    {
        var result = await _service.GetByWorkoutIdAsync(workoutId, GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Додати вправу до тренування (User,Coach, Admin).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "User,Coach,Admin")]
    [SwaggerOperation(Summary = "Додати вправу до тренування")]
    public async Task<IActionResult> Add(int workoutId, [FromBody] AddWorkoutExerciseDto dto)
    {
        await _service.AddAsync(workoutId, dto, GetUserId());
        return Ok(new { message = "Вправу додано до тренування" });
    }

    /// <summary>
    /// Видалити вправу з тренування (User,Coach, Admin).
    /// </summary>
    [HttpDelete("{exerciseId}")]
    [Authorize(Roles = "User,Coach,Admin")]
    [SwaggerOperation(Summary = "Видалити вправу з тренування")]
    public async Task<IActionResult> Remove(int workoutId, int exerciseId)
    {
        await _service.RemoveAsync(workoutId, exerciseId, GetUserId());
        return NoContent();
    }
}