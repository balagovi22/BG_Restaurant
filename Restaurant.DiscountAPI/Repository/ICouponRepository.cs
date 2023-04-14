using Restaurant.DiscountAPI.Models;
using Restaurant.DiscountAPI.Models.Dto;
using Restaurant.DiscountAPI.Models.Dtos;

namespace Restaurant.DiscountAPI.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCoupon(string CouponCode);
    }
}
