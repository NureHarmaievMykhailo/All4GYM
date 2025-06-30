using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace All4GYM.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class VideoContentController : ControllerBase
{
    private readonly IVideoContentService _service;

    public VideoContentController(IVideoContentService service)
    {
        _service = service;
    }

    /// <summary>
    /// Отримати список всіх відео.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Отримати список відео",
        Description = "Доступно для всіх авторизованих користувачів"
    )]
    public async Task<IActionResult> GetAll()
    {
        var videos = await _service.GetAllAsync();
        return Ok(videos);
    }

    /// <summary>
    /// Отримати відео за ID.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Отримати відео за ID")]
    public async Task<IActionResult> GetById(int id)
    {
        var video = await _service.GetByIdAsync(id);
        return Ok(video);
    }

    /// <summary>
    /// Додати нове відео (Coach, Admin).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Coach,Admin")]
    [SwaggerOperation(Summary = "Створити відео", Description = "Доступно лише для Coach або Admin")]
    public async Task<IActionResult> Create(CreateVideoContentDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    /// <summary>
    /// Оновити відео (Coach, Admin).
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Coach,Admin")]
    [SwaggerOperation(Summary = "Оновити відео", Description = "Доступно лише для Coach або Admin")]
    public async Task<IActionResult> Update(int id, CreateVideoContentDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return Ok(updated);
    }

    /// <summary>
    /// Видалити відео (Admin).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Видалити відео", Description = "Доступно лише для Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
