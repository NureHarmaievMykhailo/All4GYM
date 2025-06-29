using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var videos = await _service.GetAllAsync();
        return Ok(videos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var video = await _service.GetByIdAsync(id);
        return Ok(video);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateVideoContentDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateVideoContentDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}