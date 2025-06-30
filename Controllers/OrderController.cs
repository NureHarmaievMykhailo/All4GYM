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
public class OrderController : ControllerBase
{
    private readonly IOrderService _service;

    public OrderController(IOrderService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Отримати список замовлень поточного користувача.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Отримати всі замовлення користувача", Description = "Доступно для ролі User або Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync(GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Отримати конкретне замовлення за ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Отримати замовлення за ID", Description = "Доступно для власника замовлення або адміністратора")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id, GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Створити нове замовлення (User).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "User")]
    [SwaggerOperation(Summary = "Створити нове замовлення", Description = "Доступно лише для ролі User")]
    public async Task<IActionResult> Create(CreateOrderDto dto)
    {
        var created = await _service.CreateAsync(dto, GetUserId());
        return Ok(created);
    }
}