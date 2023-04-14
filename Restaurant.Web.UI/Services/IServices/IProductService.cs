using Restaurant.Web.UI.Models.Dto;

namespace Restaurant.Web.UI.Services.IServices
{
    public interface IProductService : IBaseService
    {
        Task<T> GetAllProductAsync<T>();
        
        Task<T> GetProductByIdAsync<T>(int id);

        Task<T> CreateProductAsync<T>(ProductDto product);

        Task<T> UpdateProductAsync<T>(ProductDto product);

        Task<T> DeleteProductAsync<T>(int id);
    }
}
