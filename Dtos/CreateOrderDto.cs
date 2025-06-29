namespace All4GYM.Dtos;

public class CreateOrderDto
{
    public List<OrderProductRequestDto> Products { get; set; } = new();
}