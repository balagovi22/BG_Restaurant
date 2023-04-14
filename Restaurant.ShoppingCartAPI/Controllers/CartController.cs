using Mango.MessageBus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.ShoppingCartAPI.Messages;
using Restaurant.ShoppingCartAPI.Models.Dto;
using Restaurant.ShoppingCartAPI.Repository;

namespace Restaurant.ShoppingCartAPI.Controllers
{
    [Route("api/v1/cart/")]
    [ApiController]
    public class CartControllerAPI : ControllerBase
    {
        private readonly IShoppingCartRepository _cartRepo;
        protected ShoppingCartResponseDto _responseDto;
        private readonly ICouponRepository _couponRepository;
        private readonly IMessageBus _messageBus;

        public CartControllerAPI(IShoppingCartRepository cartRepo, IMessageBus messageBus, ICouponRepository couponRepository)
        {
            _cartRepo = cartRepo;
            _responseDto = new ShoppingCartResponseDto();
            _couponRepository = couponRepository;
            _messageBus = messageBus;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                var cartDto = await _cartRepo.GetShoppingCartByUserId(userId);
                _responseDto.Result = cartDto;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _responseDto;
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart(ShoppingCartDto cart)
        {
            try
            {
                var shoppingCartDto = await _cartRepo.CreateOrUpdateCart(cart);
                _responseDto.Result = shoppingCartDto;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _responseDto;
        }
        [HttpPost("UpdateCart")]
        [Authorize]
        public async Task<object> UpdateCart(ShoppingCartDto cart)
        {
            try
            {
                var shoppingCartDto = await _cartRepo.CreateOrUpdateCart(cart);
                _responseDto.Result = shoppingCartDto;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _responseDto;
        }

        [HttpPost("RemoveCart/{cartDetailId}")]
        public async Task<object> RemoveCart([FromBody] int cartDetailId)
        {
            try
            {
                var isItemRemoved = await _cartRepo.RemoveFromCart(cartDetailId);
                _responseDto.Result = isItemRemoved;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _responseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] ShoppingCartDto cartDto)
        {
            try
            {
                var isDiscountApplied = await _cartRepo.ApplyDiscount(
                                        cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
                _responseDto.IsSuccess = isDiscountApplied;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _responseDto;
        }
        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] ShoppingCartDto cartDto)
        {
            try
            {
                var shoppingCartDto = await _cartRepo.RemoveDiscount(
                                        cartDto.CartHeader.UserId);
                _responseDto.Result = shoppingCartDto;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _responseDto;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutDto checkoutDto)
        {
            try
            {
                var cartDto = await _cartRepo.GetShoppingCartByUserId(checkoutDto.UserId);
                if (cartDto== null)
                {
                    return BadRequest();
                }
                if (!string.IsNullOrEmpty(checkoutDto.CouponCode))
                {
                    CouponDto coupon = await _couponRepository.GetCoupon(checkoutDto.CouponCode);
                    if (checkoutDto.DiscountTotal != coupon.DiscountAmount)
                    {
                        _responseDto.IsSuccess = false;
                        _responseDto.ErrorMessages = new List<string>() { "Coupon price has changed, please confirm" };
                        _responseDto.DisplayMessage = "Coupon price has changed, please confirm";
                        return _responseDto;
                    }
                }
                checkoutDto.CartDetails = cartDto.CartDetails;
                //logic to add message to process order
                //The following code is using topics to send messages
                //await _messageBus.PublishMessage(checkoutDto, "checkoutordermessagetopic");

                //Send Message to Queue instead of topic
                await _messageBus.PublishMessage(checkoutDto, "checkoutorderqueue");

                _responseDto.IsSuccess = true;
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
