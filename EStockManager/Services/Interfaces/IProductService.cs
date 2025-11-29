using EStockManager.Models.DTOs;

namespace EStockManager.Services.Interfaces
{
    public interface IProductService
    {
        // CRUD
        Task<ProductResponseDto> AddAsync(ProductCreateDto dto);
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto> GetByIdAsync(int id);
        Task UpdateAsync(int id, ProductCreateDto dto);
        Task DeleteAsync(int id);
    }
}
