using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderDto>> GetAllAsync(int userId)
    {
        var orders = await _context.Orders
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            Items = o.OrderProducts.Select(op => new OrderProductItemDto
            {
                ProductId = op.ProductId,
                ProductName = op.Product.Name,
                Price = op.Product.Price,
                Quantity = op.Quantity
            }).ToList()
        }).ToList();
    }

    public async Task<OrderDto> GetByIdAsync(int id, int userId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId)
            ?? throw new Exception("Замовлення не знайдено");

        return new OrderDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Items = order.OrderProducts.Select(op => new OrderProductItemDto
            {
                ProductId = op.ProductId,
                ProductName = op.Product.Name,
                Price = op.Product.Price,
                Quantity = op.Quantity
            }).ToList()
        };
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto, int userId)
    {
        var productIds = dto.Products.Select(p => p.ProductId).ToList();

        var products = await _context.ShopProducts
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        float total = 0f;
        var orderProducts = new List<OrderProduct>();

        foreach (var item in dto.Products)
        {
            if (!products.ContainsKey(item.ProductId))
                throw new Exception($"Товар з ID {item.ProductId} не знайдено");

            var product = products[item.ProductId];

            if (item.Quantity > product.Stock)
                throw new Exception($"Недостатньо товару {product.Name} на складі");

            product.Stock -= item.Quantity;

            total += product.Price * item.Quantity;

            orderProducts.Add(new OrderProduct
            {
                ProductId = product.Id,
                Quantity = item.Quantity
            });
        }

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = total,
            OrderProducts = orderProducts
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(order.Id, userId);
    }
}
