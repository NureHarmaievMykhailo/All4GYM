using All4GYM.Dtos;

namespace All4GYM.Services;

public class NutritionService : INutritionService
{
    public NutritionCalculationResultDto CalculateNeeds(NutritionCalculationRequestDto input)
    {
        float bmr;

        if (input.Gender.ToLower() == "male")
        {
            bmr = 10 * input.WeightKg + 6.25f * input.HeightCm - 5 * input.Age + 5;
        }
        else
        {
            bmr = 10 * input.WeightKg + 6.25f * input.HeightCm - 5 * input.Age - 161;
        }

        float activityFactor = input.ActivityLevel.ToLower() switch
        {
            "low" => 1.2f,         // Малорухливий (офіс, без спорту)
            "moderate" => 1.55f,   // Помірна активність (2–3 тренування)
            "high" => 1.75f,       // Висока активність (5–6 тренувань)
            _ => 1.5f
        };

        float tdee = bmr * activityFactor;

        float calories = input.Goal.ToLower() switch
        {
            "lose" => tdee - 500,     // Для зниження ваги
            "gain" => tdee + 400,     // Для набору маси
            _ => tdee                 // Для підтримки
        };

        // Розподіл макронутрієнтів
        float proteinGrams = (calories * 0.30f) / 4;
        float fatGrams = (calories * 0.25f) / 9;
        float carbsGrams = (calories * 0.45f) / 4;

        return new NutritionCalculationResultDto
        {
            Calories = MathF.Round(calories),
            ProteinGrams = MathF.Round(proteinGrams),
            FatGrams = MathF.Round(fatGrams),
            CarbsGrams = MathF.Round(carbsGrams)
        };
    }
}