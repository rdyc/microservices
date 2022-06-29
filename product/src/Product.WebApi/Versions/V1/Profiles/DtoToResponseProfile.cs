using System.Collections.Generic;
using AutoMapper;
using Product.Contract.Dtos;
using Product.WebApi.Versions.V1.Models;

namespace Product.WebApi.Versions.V1.Profiles;

internal class DtoToResponseProfile : Profile
{
    public DtoToResponseProfile() : base("V1 Dto to response profile")
    {
        // attribute
        CreateMap<IEnumerable<IAttributeDto>, GetAllAttributesResponse>()
            .ForMember(dst => dst.Data, opt => opt.MapFrom(src => src));
        CreateMap<IAttributeDto, AttributeResponse>();

        // currency
        CreateMap<IEnumerable<ICurrencyDto>, GetAllCurrenciesResponse>()
            .ForMember(dst => dst.Data, opt => opt.MapFrom(src => src));
        CreateMap<ICurrencyDto, CurrencyResponse>();

        // product
        CreateMap<IEnumerable<IProductDto>, GetAllProductsResponse>()
            .ForMember(dst => dst.Data, opt => opt.MapFrom(src => src));
        CreateMap<IProductDto, ProductResponse>();
    }
}