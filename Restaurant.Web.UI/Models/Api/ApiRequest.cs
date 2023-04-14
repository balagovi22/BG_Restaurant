using Microsoft.AspNetCore.Mvc;
using static Restaurant.Web.UI.SD;

namespace Restaurant.Web.UI.Models.Api
{
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }

        public object Data { get; set; }

        public string AccessToken { get; set; }
    }
}
