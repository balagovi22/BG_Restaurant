using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Restaurant.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Restaurant.Web.Services.IServices;
using Restaurant.Web.Models.Dto;
using Newtonsoft.Json;

namespace Restaurant.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _cartService;
        private string accessToken;

        public HomeController(ILogger<HomeController> logger, 
                                    IProductService productService,
                                    IShoppingCartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> products = new();
            var response = await _productService.GetAllProductAsync<ResponseDto>();
            if (response != null && response.IsSuccess) {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            Console.WriteLine(accessToken);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
           // await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Redirects to the IDP linked to scheme
            // "OpenIdConnectDefaults.AuthenticationScheme" (oidc)
            // so it can clear its own session/cookie
            //await HttpContext.SignOutAsync(
             //   OpenIdConnectDefaults.AuthenticationScheme);

            return SignOut("Cookies","oidc");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Details(int productId)
        {
            ProductDto product = new();
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId);
            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            return View(product);
        }

        [HttpPost]
        [ActionName("Details")]
        public async Task<IActionResult> Details(ProductDto productDto)
        {
            ShoppingCartDto cartDto = new ShoppingCartDto()
            {
                CartHeader = new CartHeaderDto()
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
               }
           };

            CartDetailDto cartDetails = new CartDetailDto()
            {
                Count = productDto.Count,
                ProductId = productDto.Id
            };

            var response = await _productService.GetProductByIdAsync<ResponseDto>(productDto.Id);
            if (response!=null && response.IsSuccess) {
                cartDetails.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            List<CartDetailDto> cartDetailsDtos = new();
            cartDetailsDtos.Add(cartDetails);
            cartDto.CartDetails = cartDetailsDtos;

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var addToCartResp = await _cartService.AddToCartAsync<ResponseDto>(cartDto,accessToken);
            if (addToCartResp!=null && addToCartResp.IsSuccess) {
                return RedirectToAction(nameof(Index));
            }
            return View(productDto);
        }
    }
}