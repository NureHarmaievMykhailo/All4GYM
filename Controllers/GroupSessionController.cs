using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace All4GYM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User,Admin,Coach")]
public class GroupSessionController : ControllerBase
{
    private readonly IGroupSessionService _service;

    public GroupSessionController(IGroupSessionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Отримати список усіх групових тренувань.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Отримати всі групові сесії",
        Description = "Доступно для ролей: Admin, Coach"
    )]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Отримати групову сесію за ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Отримати сесію за ID",
        Description = "Доступно для ролей: Admin, Coach"
    )]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Створити нову групову сесію.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Coach")]
    [SwaggerOperation(
        Summary = "Створити нову групову сесію",
        Description = "Доступно для ролей: Admin, Coach"
    )]
    public async Task<IActionResult> Create([FromBody] CreateGroupSessionDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    /// <summary>
    /// Оновити наявну групову сесію.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Coach")]
    [SwaggerOperation(
        Summary = "Оновити групову сесію",
        Description = "Доступно для ролей: Admin, Coach"
    )]
    public async Task<IActionResult> Update(int id, [FromBody] CreateGroupSessionDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return Ok(updated);
    }

    /// <summary>
    /// Видалити групову сесію.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Coach")]
    [SwaggerOperation(
        Summary = "Видалити групову сесію",
        Description = "Доступно лише для ролей: Admin, Coach"
    )]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
