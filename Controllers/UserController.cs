using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace All4GYM.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Реєстрація нового користувача.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Реєстрація", Description = "Доступно всім")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        try
        {
            var user = await _userService.RegisterAsync(dto);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Вхід користувача та отримання JWT-токена.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Авторизація", Description = "Доступно всім")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _userService.LoginAsync(dto);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Отримати дані профілю поточного користувача.
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    [SwaggerOperation(Summary = "Отримати профіль", Description = "Доступно авторизованому користувачу")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _userService.GetByIdAsync(userId);
        return Ok(user);
    }

    /// <summary>
    /// Оновити дані профілю поточного користувача.
    /// </summary>
    [HttpPut("profile")]
    [Authorize]
    [SwaggerOperation(Summary = "Оновити профіль", Description = "Доступно авторизованому користувачу")]
    public async Task<IActionResult> UpdateProfile([FromBody] RegisterUserDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var updated = await _userService.UpdateAsync(userId, dto);
        return Ok(updated);
    }

    /// <summary>
    /// Видалити свій обліковий запис.
    /// </summary>
    [HttpDelete("delete")]
    [Authorize]
    [SwaggerOperation(Summary = "Видалити свій акаунт", Description = "Доступно авторизованому користувачу")]
    public async Task<IActionResult> DeleteOwnAccount()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _userService.DeleteAsync(userId);
        return NoContent();
    }

    /// <summary>
    /// Отримати список усіх користувачів (тільки Admin).
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin,SystemAdmin")]
    [SwaggerOperation(Summary = "Отримати всіх користувачів", Description = "Доступно лише для ролі Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
}
