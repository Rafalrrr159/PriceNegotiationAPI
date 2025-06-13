using AutoMapper;
using PriceNegotiationAPI.DTOs.Negotiation;
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

            CreateMap<CreateNegotiationDto, Negotiation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProposedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.AttemptsCount, opt => opt.Ignore())
                .ForMember(dest => dest.LastRejectionDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedDate, opt => opt.Ignore());

            CreateMap<Negotiation, NegotiationDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
