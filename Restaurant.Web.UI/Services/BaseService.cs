using Newtonsoft.Json;
using Restaurant.Web.UI.Models.Api;
using Restaurant.Web.UI.Models.Dto;
using Restaurant.Web.UI.Services.IServices;
using System.Text;

namespace Restaurant.Web.UI.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _clientFactory;
        public BaseService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            responseModel = new ResponseDto();
        }
        public ResponseDto responseModel { get; set; }
        public async Task<T> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                var client = _clientFactory.CreateClient("ProductAPI");
                client.DefaultRequestHeaders.Clear();

                //Set message property values
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                AssignApiType(apiRequest, message);
                if (apiRequest.Data != null) //Needed for POST, PUT when data is being sent
                {
                    message.Content = new StringContent(
                                            JsonConvert.SerializeObject(apiRequest.Data),
                                            Encoding.UTF8, "application/json");
                }

                HttpResponseMessage apiResponse = null;
                apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                var apiResponseDto = JsonConvert.DeserializeObject<T>(apiContent);
                return apiResponseDto;
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    DisplayMessage = "Error",
                    ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccess = false
                };
                var response = JsonConvert.SerializeObject(dto);
                var apiResponseDto = JsonConvert.DeserializeObject<T>(response);
                return apiResponseDto;
            }

            static void AssignApiType(ApiRequest apiRequest, HttpRequestMessage message)
            {
                switch (apiRequest.ApiType)
                {
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
            }
        }
     
        public void Dispose()
        {
            GC.SuppressFinalize(true);
        }
    }
}
