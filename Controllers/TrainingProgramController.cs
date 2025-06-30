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
public class TrainingProgramController : ControllerBase
{
    private readonly ITrainingProgramService _service;

    public TrainingProgramController(ITrainingProgramService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Отримати список тренувальних програм користувача.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Отримати всі тренувальні програми",
        Description = "Доступно для ролей: User, Coach, Admin"
    )]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync(GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Отримати тренувальну програму за ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Отримати тренувальну програму за ID",
        Description = "Доступно для ролей: User, Coach, Admin"
    )]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id, GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Створити нову тренувальну програму.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Coach,Admin")]
    [SwaggerOperation(
        Summary = "Створити нову програму",
        Description = "Доступно для ролей: Coach, Admin"
    )]
    public async Task<IActionResult> Create(CreateTrainingProgramDto dto)
    {
        var result = await _service.CreateAsync(dto, GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Оновити тренувальну програму.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Coach,Admin")]
    [SwaggerOperation(
        Summary = "Оновити програму",
        Description = "Доступно для ролей: Coach, Admin"
    )]
    public async Task<IActionResult> Update(int id, CreateTrainingProgramDto dto)
    {
        var result = await _service.UpdateAsync(id, dto, GetUserId());
        return Ok(result);
    }

    /// <summary>
    /// Видалити тренувальну програму.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Видалити програму",
        Description = "Доступно лише для ролі Admin"
    )]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, GetUserId());
        return NoContent();
    }
}
