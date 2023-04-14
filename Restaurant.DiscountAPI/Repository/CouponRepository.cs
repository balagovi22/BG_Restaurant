using AutoMapper;
using Restaurant.DiscountAPI.Database;
using Restaurant.DiscountAPI.Models.Dto;
using Restaurant.DiscountAPI.Models.Dtos;

namespace Restaurant.DiscountAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public CouponRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<CouponDto> GetCoupon(string couponCode)
        {
            var coupon = _db.Coupons.FirstOrDefault(c => c.CouponCode == couponCode);
            if (coupon != null)
            {
                return _mapper.Map<CouponDto>(coupon);
            }
            return null;
        }
    }
}
