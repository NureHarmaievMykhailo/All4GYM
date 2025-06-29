namespace All4GYM.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public float TotalAmount { get; set; }

    public List<OrderProductItemDto> Items { get; set; } = new();
}