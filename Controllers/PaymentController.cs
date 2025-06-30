using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace All4GYM.Controllers;

[ApiController]
[Authorize(Roles = "User")]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentController(IPaymentService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Створити Stripe-платіж для замовлення.
    /// </summary>
    /// <param name="dto">OrderId замовлення</param>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Ініціювати оплату",
        Description = "Доступно лише для ролі User. Повертає Stripe ClientSecret для клієнтської оплати."
    )]
    public async Task<IActionResult> Create(CreatePaymentDto dto)
    {
        var result = await _service.CreateAsync(dto, GetUserId());
        return Ok(result);
    }
}