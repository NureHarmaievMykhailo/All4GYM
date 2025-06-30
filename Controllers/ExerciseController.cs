using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace All4GYM.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ExerciseController : ControllerBase
{
    private readonly IExerciseService _service;

    public ExerciseController(IExerciseService service)
    {
        _service = service;
    }

    /// <summary>
    /// Отримати список усіх вправ.
    /// </summary>
    /// <returns>Список об'єктів вправ</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Отримати всі вправи", Description = "Доступно для всіх авторизованих користувачів")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Отримати вправу за її ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Отримати вправу за ID")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Створити нову вправу (тільки для тренерів або адміністраторів).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Coach,SystemAdmin")]
    [SwaggerOperation(Summary = "Створити вправу", Description = "Доступно лише для ролей Admin або Trainer")]
    public async Task<IActionResult> Create(CreateExerciseDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return Ok(result);
    }

    /// <summary>
    /// Оновити вправу за її ID (тільки для тренерів або адміністраторів).
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Coach,SystemAdmin")]
    [SwaggerOperation(Summary = "Оновити вправу", Description = "Доступно лише для ролей Admin або Trainer")]
    public async Task<IActionResult> Update(int id, CreateExerciseDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Видалити вправу за її ID (тільки для адміністраторів).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SystemAdmin")]
    [SwaggerOperation(Summary = "Видалити вправу", Description = "Доступно лише для ролі Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
