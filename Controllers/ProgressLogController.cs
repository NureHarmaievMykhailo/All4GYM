using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace All4GYM.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProgressLogController : ControllerBase
{
    private readonly IProgressLogService _service;

    public ProgressLogController(IProgressLogService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var logs = await _service.GetAllAsync(GetUserId());
        return Ok(logs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var log = await _service.GetByIdAsync(id, GetUserId());
        return Ok(log);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProgressLogDto dto)
    {
        var created = await _service.CreateAsync(dto, GetUserId());
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateProgressLogDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto, GetUserId());
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, GetUserId());
        return NoContent();
    }
}