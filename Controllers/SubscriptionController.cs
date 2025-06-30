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
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _service;

    public SubscriptionController(ISubscriptionService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Отримати всі підписки поточного користувача.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Отримати всі підписки користувача",
        Description = "Доступно для ролей: User, Admin"
    )]
    public async Task<IActionResult> GetAll()
    {
        var list = await _service.GetAllAsync(GetUserId());
        return Ok(list);
    }

    /// <summary>
    /// Отримати підписку за ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Отримати підписку за ID",
        Description = "Доступно для ролей: User, Admin"
    )]
    public async Task<IActionResult> GetById(int id)
    {
        var sub = await _service.GetByIdAsync(id, GetUserId());
        return Ok(sub);
    }

    /// <summary>
    /// Оформити нову підписку (тільки для User).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "User")]
    [SwaggerOperation(
        Summary = "Оформити підписку",
        Description = "Доступно лише для ролі User"
    )]
    public async Task<IActionResult> Create(CreateSubscriptionDto dto)
    {
        var sub = await _service.CreateAsync(dto, GetUserId());
        return Ok(sub);
    }

    /// <summary>
    /// Оновити підписку (тільки Admin).
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Оновити підписку",
        Description = "Доступно лише для ролі Admin"
    )]
    public async Task<IActionResult> Update(int id, CreateSubscriptionDto dto)
    {
        var sub = await _service.UpdateAsync(id, dto, GetUserId());
        return Ok(sub);
    }

    /// <summary>
    /// Видалити підписку (User або Admin).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "User,Admin")]
    [SwaggerOperation(
        Summary = "Видалити підписку",
        Description = "Доступно для ролей: User, Admin"
    )]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, GetUserId());
        return NoContent();
    }
}
