using All4GYM.Dtos;

namespace All4GYM.Services;

public interface INutritionService
{
    NutritionCalculationResultDto CalculateNeeds(NutritionCalculationRequestDto input);
}