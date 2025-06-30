using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace All4GYM.Controllers;

[ApiController]
[Authorize(Roles = "User,Coach,Admin")]
[Route("api/[controller]")]
public class WorkoutController : ControllerBase
{
    private readonly IWorkoutService _service;

    public WorkoutController(IWorkoutService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Отримати всі тренування користувача.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Отримати список тренувань")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync(GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Отримати тренування за ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Отримати тренування за ID")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id, GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Створити тренування (Coach, Admin).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Coach,Admin")]
    [SwaggerOperation(Summary = "Створити тренування")]
    public async Task<IActionResult> Create(CreateWorkoutDto dto)
    {
        var result = await _service.CreateAsync(dto, GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Оновити тренування (Coach, Admin).
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Coach,Admin")]
    [SwaggerOperation(Summary = "Оновити тренування")]
    public async Task<IActionResult> Update(int id, CreateWorkoutDto dto)
    {
        var result = await _service.UpdateAsync(id, dto, GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Видалити тренування (Admin).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Видалити тренування")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, GetUserId());
        return NoContent();
    }
}
