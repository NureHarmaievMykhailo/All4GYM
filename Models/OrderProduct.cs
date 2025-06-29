namespace All4GYM.Models;

public class OrderProduct
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public ShopProduct Product { get; set; } = null!;

    public int Quantity { get; set; }
}