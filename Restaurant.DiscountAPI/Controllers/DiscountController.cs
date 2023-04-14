using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.DiscountAPI.Models.Dto;
using Restaurant.DiscountAPI.Repository;

namespace Restaurant.DiscountAPI.Controllers
{
    [Route("api/v1/coupon/")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly ICouponRepository _couponRepo;
        protected DiscountResponseDto _responseDto;

        public DiscountController(ICouponRepository couponRepo)
        {
            _couponRepo = couponRepo;
            _responseDto = new DiscountResponseDto();
        }

        [HttpGet("{couponCode}")]
        public async Task<object> GetCoupon(string couponCode)
        {
            try
            {
                var couponDto = await _couponRepo.GetCoupon(couponCode);
                _responseDto.Result = couponDto;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _responseDto;
        }
    }
}
