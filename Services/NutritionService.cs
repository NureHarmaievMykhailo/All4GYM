using All4GYM.Dtos;
using All4GYM.Dtos.Enums;

namespace All4GYM.Services;

public class NutritionService : INutritionService
{
    // Константы калорийности макронутриентов
    private const int CALORIES_PER_GRAM_PROTEIN = 4;
    private const int CALORIES_PER_GRAM_CARBS = 4;
    private const int CALORIES_PER_GRAM_FAT = 9;

    public NutritionCalculationResultDto CalculateNeeds(NutritionCalculationRequestDto input)
    {
        double bmr = (10 * input.WeightKg) + (6.25 * input.HeightCm) - (5 * input.Age);
        bmr += input.Gender == GenderType.Male ? 5 : -161;
        
        double activityMultiplier = input.Activity switch
        {
            ActivityLevel.Sedentary => 1.2,
            ActivityLevel.LightlyActive => 1.375,
            ActivityLevel.ModeratelyActive => 1.55,
            ActivityLevel.VeryActive => 1.725,
            ActivityLevel.ExtraActive => 1.9,
            _ => 1.2
        };
        
        double tdee = bmr * activityMultiplier;
        
        double targetCalories = tdee;
        double proteinProp = 0.30; 
        double fatProp = 0.30;     

        switch (input.Goal)
        {
            case FitnessGoal.LoseFast:
                targetCalories = tdee * 0.80;
                proteinProp = 0.40;
                fatProp = 0.25;
                break;
            case FitnessGoal.LoseSlow:
                targetCalories = tdee * 0.90;
                proteinProp = 0.35;
                fatProp = 0.25;
                break;
            case FitnessGoal.Gain:
                targetCalories = tdee * 1.15;
                proteinProp = 0.30;
                fatProp = 0.25;
                break;
            case FitnessGoal.Maintain:
            default:
                targetCalories = tdee;
                proteinProp = 0.30;
                fatProp = 0.30;
                break;
        }
        
        double minCalories = input.Gender == GenderType.Male ? 1500 : 1200;
        if (targetCalories < minCalories) targetCalories = minCalories;

        // 5. Расчет макронутриентов в граммах
        int proteins = (int)Math.Round((targetCalories * proteinProp) / CALORIES_PER_GRAM_PROTEIN);
        int fats = (int)Math.Round((targetCalories * fatProp) / CALORIES_PER_GRAM_FAT);
        
        int carbsCal = (int)targetCalories - (proteins * CALORIES_PER_GRAM_PROTEIN) - (fats * CALORIES_PER_GRAM_FAT);
        int carbs = Math.Max(0, (int)Math.Round((double)carbsCal / CALORIES_PER_GRAM_CARBS));
        
        double heightInMeters = input.HeightCm / 100.0;
        double bmi = input.WeightKg / (heightInMeters * heightInMeters);
        
        string bmiStatus = bmi switch
        {
            < 18.5 => "Недостатня вага",
            >= 18.5 and < 25 => "Норма",
            >= 25 and < 30 => "Надмірна вага",
            _ => "Ожиріння"
        };
        
        double healthyWeightMin = 18.5 * (heightInMeters * heightInMeters);
        double healthyWeightMax = 24.9 * (heightInMeters * heightInMeters);
        
        double weightDiff = 0;
        if (input.WeightKg > healthyWeightMax)
        {
            weightDiff = healthyWeightMax - input.WeightKg;
        }
        else if (input.WeightKg < healthyWeightMin)
        {
            weightDiff = healthyWeightMin - input.WeightKg;
        }
        
        return new NutritionCalculationResultDto
        {
            Bmr = (int)Math.Round(bmr),
            Tdee = (int)Math.Round(tdee),
            TargetCalories = (int)Math.Round(targetCalories),
            Bmi = Math.Round(bmi, 1),
            BmiStatus = bmiStatus,
            HealthyWeightMin = Math.Round(healthyWeightMin, 1),
            HealthyWeightMax = Math.Round(healthyWeightMax, 1),
            WeightDifference = Math.Round(weightDiff, 1),
            TargetProteins = proteins,
            TargetFats = fats,
            TargetCarbs = carbs
        };
    }
}