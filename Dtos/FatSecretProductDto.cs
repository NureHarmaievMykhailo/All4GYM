namespace All4GYM.Dtos;

public class FatSecretProductDto
{
    public long FoodId { get; set; }
    public string FoodName { get; set; } = null!;
    public string BrandName { get; set; } = string.Empty;
    public string FoodDescription { get; set; } = null!;
}