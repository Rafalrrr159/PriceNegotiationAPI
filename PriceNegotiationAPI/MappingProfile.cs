using AutoMapper;
using PriceNegotiationAPI.DTOs.Product;
using PriceNegotiationAPI.Models;

namespace PriceNegotiationAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null));

            CreateMap<Product, ProductDto>();
        }
    }
}
