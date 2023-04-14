using AutoMapper;
using Restaurant.OrderAPI.Messages;
using Restaurant.OrderAPI.Models;

namespace Restaurant.OrderAPI.MappingProfile
{
    public class MappingProfileConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CheckoutDto, OrderHeader>().ReverseMap();
                
            });
            return mappingConfig;
        }
    }
}
