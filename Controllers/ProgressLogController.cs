using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace All4GYM.Controllers;

[ApiController]
[Authorize(Roles = "User,Admin")]
[Route("api/[controller]")]
public class ProgressLogController : ControllerBase
{
    private readonly IProgressLogService _service;

    public ProgressLogController(IProgressLogService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Отримати всі записи прогресу користувача.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Отримати список записів прогресу",
        Description = "Доступно для ролей: User, Admin"
    )]
    public async Task<IActionResult> GetAll()
    {
        var logs = await _service.GetAllAsync(GetUserId());
        return Ok(logs);
    }

    /// <summary>
    /// Отримати конкретний запис прогресу за ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Отримати запис за ID",
        Description = "Доступно для ролей: User, Admin"
    )]
    public async Task<IActionResult> GetById(int id)
    {
        var log = await _service.GetByIdAsync(id, GetUserId());
        return Ok(log);
    }

    /// <summary>
    /// Створити новий запис прогресу.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "User")]
    [SwaggerOperation(
        Summary = "Створити запис прогресу",
        Description = "Доступно лише для ролі User"
    )]
    public async Task<IActionResult> Create(CreateProgressLogDto dto)
    {
        var created = await _service.CreateAsync(dto, GetUserId());
        return Ok(created);
    }

    /// <summary>
    /// Оновити запис прогресу.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "User")]
    [SwaggerOperation(
        Summary = "Оновити запис прогресу",
        Description = "Доступно лише для ролі User"
    )]
    public async Task<IActionResult> Update(int id, CreateProgressLogDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto, GetUserId());
        return Ok(updated);
    }

    /// <summary>
    /// Видалити запис прогресу.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "User,Admin")]
    [SwaggerOperation(
        Summary = "Видалити запис прогресу",
        Description = "Доступно для ролей: User, Admin"
    )]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, GetUserId());
        return NoContent();
    }
}
