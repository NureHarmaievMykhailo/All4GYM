using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace All4GYM.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ShopController : ControllerBase
{
    private readonly IShopService _service;

    public ShopController(IShopService service)
    {
        _service = service;
    }

    /// <summary>
    /// Отримати список усіх товарів у магазині.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Отримати всі товари",
        Description = "Доступно для всіх авторизованих користувачів"
    )]
    public async Task<IActionResult> GetAll()
    {
        var list = await _service.GetAllAsync();
        return Ok(list);
    }

    /// <summary>
    /// Отримати товар за його ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Отримати товар за ID",
        Description = "Доступно для всіх авторизованих користувачів"
    )]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return Ok(product);
    }

    /// <summary>
    /// Створити новий товар у магазині (тільки Admin).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Створити товар",
        Description = "Доступно лише для ролі Admin"
    )]
    public async Task<IActionResult> Create(CreateShopProductDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    /// <summary>
    /// Оновити товар у магазині (тільки Admin).
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Оновити товар",
        Description = "Доступно лише для ролі Admin"
    )]
    public async Task<IActionResult> Update(int id, CreateShopProductDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return Ok(updated);
    }

    /// <summary>
    /// Видалити товар з магазину (тільки Admin).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Видалити товар",
        Description = "Доступно лише для ролі Admin"
    )]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
