using AutoMapper;
using Restaurant.DiscountAPI.Models;
using Restaurant.DiscountAPI.Models.Dto;
using Restaurant.DiscountAPI.Models.Dtos;

namespace Restaurant.DiscountAPI.MappingProfile
{
    public class MappingProfileConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
