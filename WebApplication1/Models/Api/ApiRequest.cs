using Microsoft.AspNetCore.Mvc;
using static Restaurant.Web.SD;

namespace Restaurant.Web.Models.Api
{
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }

        public object Data { get; set; }

        public string AccessToken { get; set; }
    }
}
