using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class ShopService : IShopService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ShopService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ShopProductDto>> GetAllAsync()
    {
        var products = await _context.ShopProducts.ToListAsync();
        return _mapper.Map<List<ShopProductDto>>(products);
    }

    public async Task<ShopProductDto> GetByIdAsync(int id)
    {
        var product = await _context.ShopProducts.FindAsync(id)
                      ?? throw new Exception("Товар не знайдено");

        return _mapper.Map<ShopProductDto>(product);
    }

    public async Task<ShopProductDto> CreateAsync(CreateShopProductDto dto)
    {
        var product = _mapper.Map<ShopProduct>(dto);
        _context.ShopProducts.Add(product);
        await _context.SaveChangesAsync();
        return _mapper.Map<ShopProductDto>(product);
    }

    public async Task<ShopProductDto> UpdateAsync(int id, CreateShopProductDto dto)
    {
        var product = await _context.ShopProducts.FindAsync(id)
                      ?? throw new Exception("Товар не знайдено");

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;

        await _context.SaveChangesAsync();
        return _mapper.Map<ShopProductDto>(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.ShopProducts.FindAsync(id)
                      ?? throw new Exception("Товар не знайдено");

        _context.ShopProducts.Remove(product);
        await _context.SaveChangesAsync();
    }
}