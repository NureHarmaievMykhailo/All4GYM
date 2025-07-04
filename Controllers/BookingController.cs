using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace All4GYM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _service;

    public BookingController(IBookingService service)
    {
        _service = service;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Отримати всі записи на групові тренування користувача.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Отримати записи користувача")]
    public async Task<IActionResult> GetUserBookings()
    {
        var userId = GetUserId();
        var bookings = await _service.GetUserBookingsAsync(userId);
        return Ok(bookings);
    }

    /// <summary>
    /// Записатися на групове тренування.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Запис на групове тренування")]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
    {
        try
        {
            var userId = GetUserId();
            var created = await _service.CreateBookingAsync(dto, userId);
            return Ok(created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Помилка: {ex.Message}" });
        }
    }

    /// <summary>
    /// Скасувати запис на тренування.
    /// </summary>
    [HttpDelete("{sessionId}")]
    [SwaggerOperation(Summary = "Скасувати запис на групову сесію")]
    public async Task<IActionResult> Cancel(int sessionId)
    {
        try
        {
            var userId = GetUserId();
            await _service.CancelBookingAsync(sessionId, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Помилка: {ex.Message}" });
        }
    }
}
