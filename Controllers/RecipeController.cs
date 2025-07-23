using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace All4GYM.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly IRecipeService _service;

    public RecipeController(IRecipeService service) => _service = service;

    /// <summary>
    /// Отримати всі рецепти (доступно всім авторизованим користувачам)
    /// </summary>
    [HttpGet]
    [Authorize]
    [SwaggerOperation(Summary = "Отримати рецепти")]
    public async Task<IActionResult> Get()
        => Ok(await _service.GetAllAsync());

    /// <summary>
    /// Додати новий рецепт (Admin або Nutritionist).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Nutritionist")]
    [SwaggerOperation(Summary = "Створити рецепт")]
    public async Task<IActionResult> Post(CreateRecipeDto dto)
        => Ok(await _service.CreateAsync(dto));
}