namespace All4GYM.Dtos;

public class NutritionCalculationResultDto
{
    public int Bmr { get; set; } 
    public int Tdee { get; set; } 
    public int TargetCalories { get; set; } 

    public double Bmi { get; set; }
    public string BmiStatus { get; set; } = null!; 
    public double HealthyWeightMin { get; set; }
    public double HealthyWeightMax { get; set; }

    public double WeightDifference { get; set; } 

    public int TargetProteins { get; set; }
    public int TargetFats { get; set; }
    public int TargetCarbs { get; set; }
}
