using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace All4GYM.Controllers;

[ApiController]
[Authorize(Roles = "User,Admin")]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _service;
    private readonly JwtService _jwtService;
    private readonly AppDbContext _context;

    public SubscriptionController(
        ISubscriptionService service,
        JwtService jwtService,
        AppDbContext context)
    {
        _service = service;
        _jwtService = jwtService;
        _context = context;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? throw new Exception("UserId не знайдено в токені"));

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

    /// <summary>
    /// Скасувати поточну активну підписку користувача.
    /// </summary>
    [HttpPost("cancel")]
    [Authorize(Roles = "User")]
    [SwaggerOperation(
        Summary = "Скасувати активну підписку",
        Description = "Скасовує чинну активну підписку користувача та повертає оновлений токен"
    )]
    public async Task<IActionResult> Cancel()
    {
        int userId = GetUserId();

        await _service.CancelAsync(userId);

        var updatedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (updatedUser == null)
            return Unauthorized("Користувача не знайдено");

        var newToken = _jwtService.GenerateToken(updatedUser);

        return Ok(new
        {
            message = "Підписку скасовано",
            token = newToken
        });
    }
}
