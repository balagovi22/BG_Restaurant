using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurant.Web.Models.Dto;
using Restaurant.Web.Models.Dtos;
using Restaurant.Web.Services.IServices;

namespace Restaurant.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _service;
        private readonly IShoppingCartService _cartService;
        private readonly ICouponService _couponService;
        private ResponseDto _responseDto;
        public CartController(IProductService service, 
                                IShoppingCartService cartService,
                                ICouponService couponService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _cartService = cartService;
            _couponService = couponService;
            _responseDto = new ResponseDto();
        }
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(ShoppingCartDto cartDto)
        {
            try
            {
                var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _cartService.CheckOut<ResponseDto>(cartDto.CartHeader, accessToken);
                if (!response.IsSuccess)
                {
                    TempData["Error"] = response.DisplayMessage;
                    return RedirectToAction(nameof(Checkout));
                }
                return RedirectToAction(nameof(Confirmation));
            }
            catch (Exception)
            {
                return View(cartDto);
            }
        }

        public IActionResult Confirmation()
        {
            return View();
        }

        public async Task<IActionResult> RemoveItemFromCart(int cartDetailId)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveFromCartAsync<ResponseDto>(cartDetailId, accessToken);

            ShoppingCartDto cartDto = new();
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(ShoppingCartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            cartDto.CartHeader.UserId = userId;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.ApplyCoupon<ResponseDto>(cartDto, accessToken);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(ShoppingCartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            cartDto.CartHeader.UserId = userId;
            var response = await _cartService.RemoveCoupon<ResponseDto>(cartDto, accessToken);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        private async Task<ShoppingCartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, accessToken);

            ShoppingCartDto cartDto = new();
            if (response !=null && response.IsSuccess)
            {
                cartDto = JsonConvert.DeserializeObject<ShoppingCartDto>(Convert.ToString(response.Result));
            }
            if (cartDto.CartHeader!= null)
            {
                CouponDto couponDto = new();
                if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCoupon<ResponseDto>(cartDto.CartHeader.CouponCode, 
                                                        accessToken);
                    if (coupon != null && coupon.IsSuccess)
                    {
                        couponDto = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(coupon.Result));
                        cartDto.CartHeader.DiscountTotal = couponDto.DiscountAmount;
                    }
                }
                foreach(var detail in cartDto.CartDetails)
                {
                    cartDto.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);
                }
                if (couponDto != null)
                {
                    cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
                }
            }
            return cartDto;
        }
    }
}
