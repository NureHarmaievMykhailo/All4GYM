using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace All4GYM.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MealLogController : ControllerBase
{
    private readonly IMealLogService _service;

    public MealLogController(IMealLogService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Отримати всі записи харчування користувача.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "User,Nutritionist,Admin")]
    [SwaggerOperation(Summary = "Отримати харчовий щоденник", Description = "Доступно для ролей: User, Nutritionist, Admin")]
    public async Task<IActionResult> GetAll()
    {
        var logs = await _service.GetAllAsync(GetUserId());
        return Ok(logs);
    }

    /// <summary>
    /// Отримати конкретний запис харчування за ID.
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "User,Nutritionist,Admin")]
    [SwaggerOperation(Summary = "Отримати запис за ID", Description = "Доступно для ролей: User, Nutritionist, Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var log = await _service.GetByIdAsync(id, GetUserId());
        return Ok(log);
    }

    /// <summary>
    /// Створити новий запис харчування.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "User,Nutritionist")]
    [SwaggerOperation(Summary = "Створити новий запис харчування", Description = "Доступно для ролей: User, Nutritionist")]
    public async Task<IActionResult> Create(CreateMealLogDto dto)
    {
        var created = await _service.CreateAsync(dto, GetUserId());
        return Ok(created);
    }

    /// <summary>
    /// Оновити запис харчування.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "User,Nutritionist")]
    [SwaggerOperation(Summary = "Оновити запис харчування", Description = "Доступно для ролей: User, Nutritionist")]
    public async Task<IActionResult> Update(int id, CreateMealLogDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto, GetUserId());
        return Ok(updated);
    }

    /// <summary>
    /// Видалити запис харчування.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "User,Admin")]
    [SwaggerOperation(Summary = "Видалити запис харчування", Description = "Доступно для ролей: User, Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, GetUserId());
        return NoContent();
    }
}
