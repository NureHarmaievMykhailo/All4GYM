namespace All4GYM.Dtos;

public class OrderProductItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public float Price { get; set; }
    public int Quantity { get; set; }
}