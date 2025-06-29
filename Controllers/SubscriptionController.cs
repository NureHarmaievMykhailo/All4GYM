using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace All4GYM.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _service;

    public SubscriptionController(ISubscriptionService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _service.GetAllAsync(GetUserId());
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sub = await _service.GetByIdAsync(id, GetUserId());
        return Ok(sub);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSubscriptionDto dto)
    {
        var sub = await _service.CreateAsync(dto, GetUserId());
        return Ok(sub);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateSubscriptionDto dto)
    {
        var sub = await _service.UpdateAsync(id, dto, GetUserId());
        return Ok(sub);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id, GetUserId());
        return NoContent();
    }
}