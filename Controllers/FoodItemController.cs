using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace All4GYM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User,Admin,Nutritionist")]
public class FoodItemController : ControllerBase
{
    private readonly IFoodItemService _service;

    public FoodItemController(IFoodItemService service)
    {
        _service = service;
    }

    /// <summary>
    /// Отримати список усіх продуктів.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Отримати список продуктів")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    /// <summary>
    /// Отримати продукт за ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Отримати продукт за ID")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return Ok(item);
    }

    /// <summary>
    /// Додати новий продукт.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Додати новий продукт")]
    public async Task<IActionResult> Create(CreateFoodItemDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    /// <summary>
    /// Оновити інформацію про продукт.
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Оновити продукт")]
    public async Task<IActionResult> Update(int id, CreateFoodItemDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return Ok(updated);
    }

    /// <summary>
    /// Видалити продукт за ID.
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Видалити продукт")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}