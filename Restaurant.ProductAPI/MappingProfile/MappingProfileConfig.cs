using AutoMapper;
using Restaurant.ProductAPI.Models;
using Restaurant.ProductAPI.Models.Dto;

namespace Restaurant.ProductAPI.MappingProfile
{
    public class MappingProfileConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
