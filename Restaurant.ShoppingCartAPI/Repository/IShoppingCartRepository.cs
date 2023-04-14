using Restaurant.ShoppingCartAPI.Models;
using Restaurant.ShoppingCartAPI.Models.Dto;

namespace Restaurant.ShoppingCartAPI.Repository
{
    public interface IShoppingCartRepository
    {
       Task<ShoppingCartDto> GetShoppingCartByUserId(string userId);
        Task<ShoppingCartDto> CreateOrUpdateCart(ShoppingCartDto cartDto);

        Task<bool> RemoveFromCart(int CartDetailId);

        Task<bool> ClearShoppingCart(string UserId);

        Task<bool> ApplyDiscount(string userId, string couponCode);

        Task<bool> RemoveDiscount(string userId);
    }
}
