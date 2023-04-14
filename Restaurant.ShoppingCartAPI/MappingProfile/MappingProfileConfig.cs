using AutoMapper;
using Restaurant.ShoppingCartAPI.Models;
using Restaurant.ShoppingCartAPI.Models.Dto;

namespace Restaurant.ShoppingCartAPI.MappingProfile
{
    public class MappingProfileConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap();
                config.CreateMap<CartDetailDto, CartDetail>().ReverseMap();
                config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
                config.CreateMap<ShoppingCartDto, ShoppingCart>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
