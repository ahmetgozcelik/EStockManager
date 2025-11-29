using EStockManager.Models.DTOs;
using EStockManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EStockManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController( IProductService productService )
        {
            _productService = productService;
        }

        // yetkili yetkisiz herkes tüm ürünleri listeyebilir
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok( products );
        }

        // get by id herkese erişim
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProducById(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return Ok( product );
            }catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // sadece geçerli jwt tokenı olanlar erişebilir
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateDto productDto)
        {
            var addedProduct = await _productService.AddAsync(productDto);
            return CreatedAtAction(nameof(GetProducById), new { id = addedProduct.Id }, addedProduct);
        }

        // put güncelleme sadece yetkililer
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductCreateDto productDto)
        {
            try
            {
                await _productService.UpdateAsync(id, productDto);
                return NoContent(); // başarılı güncelleme için 204 nocontent döndürülür
            }
            catch (Exception ex)
            {
                return NotFound( new { Message = ex.Message });
            }
        }

        // delete silme sadece yetkililer
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent(); // başarılı 204 nocontent
        }
    }
}
