using All4GYM.Dtos;
using All4GYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace All4GYM.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NutritionController : ControllerBase
{
    private readonly INutritionService _service;

    public NutritionController(INutritionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Розрахунок індивідуальної норми калорій і макронутрієнтів.
    /// </summary>
    [HttpPost("calculate")]
    [SwaggerOperation(
        Summary = "Розрахунок калорій та БЖВ",
        Description = "Повертає рекомендовану кількість калорій, білків, жирів та вуглеводів на добу"
    )]
    public ActionResult<NutritionCalculationResultDto> Calculate([FromBody] NutritionCalculationRequestDto input)
    {
        var result = _service.CalculateNeeds(input);
        return Ok(result);
    }
}