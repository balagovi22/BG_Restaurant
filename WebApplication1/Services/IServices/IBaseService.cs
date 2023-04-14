using Restaurant.Web.Models.Api;
using Restaurant.Web.Models.Dto;

namespace Restaurant.Web.Services.IServices
{
    public interface IBaseService : IDisposable
    {
        ResponseDto responseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest, string API);
    }
}
