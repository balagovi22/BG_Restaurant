using Restaurant.Web.Models.Dto;

namespace Restaurant.Web.Services.IServices
{
    public interface IShoppingCartService : IBaseService
    {
        Task<T> GetCartByUserIdAsync<T>(string userId, string token=null);
        Task<T> AddToCartAsync<T>(ShoppingCartDto cartDto, string token = null);
        Task<T> UpdateCartAsync<T>(ShoppingCartDto cartDto, string token = null);
        Task<T> RemoveFromCartAsync<T>(int cartDetailId, string token = null);

        Task<T> ApplyCoupon<T>(ShoppingCartDto cartDto, string token=null);

        Task<T> RemoveCoupon<T>(ShoppingCartDto cartDto, string token = null);

        Task<T> CheckOut<T>(CartHeaderDto cartHeaderDto, string token=null);
    }
}
