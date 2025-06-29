using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IShopService
{
    Task<List<ShopProductDto>> GetAllAsync();
    Task<ShopProductDto> GetByIdAsync(int id);
    Task<ShopProductDto> CreateAsync(CreateShopProductDto dto);
    Task<ShopProductDto> UpdateAsync(int id, CreateShopProductDto dto);
    Task DeleteAsync(int id);
}