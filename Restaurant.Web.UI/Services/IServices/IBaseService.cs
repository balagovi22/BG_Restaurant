using Restaurant.Web.UI.Models.Api;
using Restaurant.Web.UI.Models.Dto;

namespace Restaurant.Web.UI.Services.IServices
{
    public interface IBaseService : IDisposable
    {
        ResponseDto responseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
