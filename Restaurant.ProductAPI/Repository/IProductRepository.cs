using Restaurant.ProductAPI.Models.Dto;
using Restaurant.ProductAPI.Models;

namespace Restaurant.ProductAPI.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetAllProducts();
        Task<ProductDto> GetProductById(int id);
        Task<ProductDto> AddModifyProduct(ProductDto productDto);
        Task<bool> DeleteProduct(int productId);
    }
}
