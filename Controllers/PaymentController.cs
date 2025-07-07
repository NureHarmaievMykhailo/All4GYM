using All4GYM.Services.Stripe;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace All4GYM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User")]
public class CheckoutController : ControllerBase
{
    private readonly StripePaymentIntentService _stripeService;

    public CheckoutController(StripePaymentIntentService stripeService)
    {
        _stripeService = stripeService;
    }

    private string GetUserEmail() =>
        User.FindFirstValue(ClaimTypes.Email)
        ?? throw new Exception("Email користувача не знайдено");

    /// <summary>
    /// Створити Stripe Checkout-сесію для підписки.
    /// </summary>
    /// <param name="tier">Тип підписки: basic, pro, premium</param>
    [HttpPost("{tier}")]
    [SwaggerOperation(
        Summary = "Створити Stripe Checkout-сесію",
        Description = "Повертає URL для перенаправлення користувача до Stripe Checkout"
    )]
    public async Task<IActionResult> Create(string tier)
    {
        var email = GetUserEmail();
        var successUrl = "http://localhost:5263/SubscriptionSuccess";
        var cancelUrl = "http://localhost:5263/SubscriptionCancel";

        try
        {
            var sessionUrl = await _stripeService.CreateCheckoutSessionAsync(email, tier, successUrl, cancelUrl);
            return Ok(new { url = sessionUrl });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Помилка створення сесії: {ex.Message}" });
        }
    }
}