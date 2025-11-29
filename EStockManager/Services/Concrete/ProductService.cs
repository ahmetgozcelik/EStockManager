using EStockManager.Models.DTOs;
using EStockManager.Models.Entities;
using EStockManager.Repositories.Interfaces;
using EStockManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EStockManager.Services.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepository;

        public ProductService(IGenericRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        // create
        public async Task<ProductResponseDto> AddAsync(ProductCreateDto dto)
        {
            // dto dan entity e ( veritabanı varlığına ) dönüşüm
            var productEntity = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
            };

            await _productRepository.AddAsync(productEntity);
            await _productRepository.SaveChangesAsync();

            // eklenen entity'den response dto'ya dönüşüm
            return new ProductResponseDto
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Description = productEntity.Description,
                Price = productEntity.Price,
                StockQuantity = productEntity.StockQuantity,
            };
        }

        // read
        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            return await _productRepository.GetAll()
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                }).ToListAsync();
        }

        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            var productEntity = await _productRepository.GetByIdAsync(id);

            if(productEntity == null)
            {
                throw new Exception("Ürün bulunamadı.");
            }

            return new ProductResponseDto
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Description = productEntity.Description,
                Price = productEntity.Price,
                StockQuantity = productEntity.StockQuantity,
            };
        }

        // update
        public async Task UpdateAsync(int id, ProductCreateDto dto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);

            if(existingProduct == null)
            {
                throw new Exception("Güncellenecek ürün bulunamadı.");
            }

            existingProduct.Name = dto.Name;
            existingProduct.Description = dto.Description;
            existingProduct.Price = dto.Price;
            existingProduct.StockQuantity = dto.StockQuantity;

            _productRepository.Update(existingProduct); // ef core takibi başlar
            await _productRepository.SaveChangesAsync();
        }

        // delete
        public async Task DeleteAsync(int id)
        {
            var productEntity = await _productRepository.GetByIdAsync(id);

            if (productEntity == null)
            {
                return;
            }

            _productRepository.Remove(productEntity);
            await _productRepository.SaveChangesAsync();
        }
    }
}
