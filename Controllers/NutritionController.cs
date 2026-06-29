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
public class NutritionController : ControllerBase
{
    private readonly INutritionService _service;
    private readonly IUserService _userService;

    public NutritionController(INutritionService service, IUserService userService)
    {
        _service = service;
        _userService = userService;
    }

    /// <summary>
    /// Розрахунок індивідуальної норми калорій і макронутрієнтів із автоматичним збереженням у профіль.
    /// </summary>
    [HttpPost("calculate")]
    [SwaggerOperation(
        Summary = "Розрахунок калорій та БЖВ",
        Description = "Повертає рекомендовану кількість калорій, білків, жирів та вуглеводів на добу та автоматично зберігає їх у базу користувача"
    )]
    public async Task<ActionResult<NutritionCalculationResultDto>> Calculate([FromBody] NutritionCalculationRequestDto input)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Користувач не авторизований або токен недійсний" });
            }

            var result = _service.CalculateNeeds(input);

            var currentUser = await _userService.GetByIdAsync(userId);

            var updateDto = new UpdateUserProfileDto
            {
                FullName = currentUser.FullName,
                Email = currentUser.Email,

                TargetCalories = result.TargetCalories,
                TargetProteins = result.TargetProteins,
                TargetFats = result.TargetFats,
                TargetCarbs = result.TargetCarbs,
                
                Age = input.Age,
                HeightCm = input.HeightCm,
                WeightKg = input.WeightKg,
                Gender = input.Gender.ToString(),
                Goal = input.Goal.ToString()
            };
            
            await _userService.UpdateAsync(userId, updateDto);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}