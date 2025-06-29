namespace All4GYM.Dtos;

public class CreateShopProductDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public float Price { get; set; }
    public int Stock { get; set; }
}