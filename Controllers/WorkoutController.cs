using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace All4GYM.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class WorkoutController : ControllerBase
{
    private readonly IWorkoutService _service;

    public WorkoutController(IWorkoutService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync(GetUserId());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id, GetUserId());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkoutDto dto)
    {
        var result = await _service.CreateAsync(dto, GetUserId());
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateWorkoutDto dto)
    {
        var result = await _service.UpdateAsync(id, dto, GetUserId());
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, GetUserId());
        return NoContent();
    }
}