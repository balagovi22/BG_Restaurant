using Restaurant.ShoppingCartAPI.Models.Dto;

namespace Restaurant.ShoppingCartAPI.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCoupon(string couponName);
    }
}
