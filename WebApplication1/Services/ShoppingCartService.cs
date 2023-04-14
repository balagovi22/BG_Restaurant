using Restaurant.Web.Models.Api;
using Restaurant.Web.Models.Dto;
using Restaurant.Web.Services.IServices;

namespace Restaurant.Web.Services
{
    public class ShoppingCartService : BaseService, IShoppingCartService
    {

        private readonly IHttpClientFactory _clientFactory;

        public ShoppingCartService(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public ResponseDto responseModel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task<T> AddToCartAsync<T>(ShoppingCartDto cartDto, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBaseURL + "/api/v1/cart/AddCart",
                AccessToken = token
            },"ShoppingCartAPI");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<T> GetCartByUserIdAsync<T>(string userId, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ShoppingCartAPIBaseURL + "/api/v1/cart/GetCart/" + userId,
                AccessToken = token
            });
        }

        public async Task<T> RemoveFromCartAsync<T>(int cartDetailId, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartAPIBaseURL + "/api/v1/cart/RemoveCart/" + cartDetailId,
                AccessToken = token
            });
        }

        public async Task<T> UpdateCartAsync<T>(ShoppingCartDto cartDto, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBaseURL + "/api/v1/cart/UpdateCart",
                AccessToken = token
            });
        }

        public async Task<T> ApplyCoupon<T>(ShoppingCartDto cartDto, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data= cartDto,
                Url = SD.ShoppingCartAPIBaseURL + "/api/v1/cart/ApplyCoupon",
                AccessToken = token
            });
        }

        public async Task<T> RemoveCoupon<T>(ShoppingCartDto cartDto, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data= cartDto,
                Url = SD.ShoppingCartAPIBaseURL + "/api/v1/cart/RemoveCoupon",
                AccessToken = token
            });
        }

        public async Task<T> CheckOut<T>(CartHeaderDto cartHeaderDto, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = cartHeaderDto,
                Url = SD.ShoppingCartAPIBaseURL + "/api/v1/cart/Checkout",
                AccessToken = token
            }, "ShoppingCartAPI");
        }
    }
}
