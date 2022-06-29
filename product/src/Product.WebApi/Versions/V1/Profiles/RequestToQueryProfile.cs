using System;
using AutoMapper;
using Product.Contract.Queries;
using Product.WebApi.Versions.V1.Converters;
using Product.WebApi.Versions.V1.Models;

namespace Product.WebApi.Versions.V1.Profiles;

internal class RequestToQueryProfile : Profile
{
    public RequestToQueryProfile() : base("V1 Request to query profile")
    {
        // attribute
        CreateMap<GetAllAttributesRequest, GetAllAttributesQuery>()
            .ForMember(dst => dst.Criteria, opt => opt.ConvertUsing<CriteriaConverter, GetAllAttributesRequest>(src => src))
            .ForMember(dst => dst.Ordered, opt => opt.ConvertUsing<OrderedConverter, GetAllAttributesRequest>(src => src))
            .ForMember(dst => dst.Paged, opt => opt.ConvertUsing<PagedConverter, GetAllAttributesRequest>(src => src));

        CreateMap<Guid, GetAttributeQuery>()
            .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src));

        // attribute
        CreateMap<GetAllCurrenciesRequest, GetAllCurrenciesQuery>()
            .ForMember(dst => dst.Criteria, opt => opt.ConvertUsing<CriteriaConverter, GetAllCurrenciesRequest>(src => src))
            .ForMember(dst => dst.Ordered, opt => opt.ConvertUsing<OrderedConverter, GetAllCurrenciesRequest>(src => src))
            .ForMember(dst => dst.Paged, opt => opt.ConvertUsing<PagedConverter, GetAllCurrenciesRequest>(src => src));

        CreateMap<Guid, GetCurrencyQuery>()
            .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src));

        // product
        CreateMap<GetAllProductsRequest, GetAllProductsQuery>()
            .ForMember(dst => dst.Criteria, opt => opt.ConvertUsing<CriteriaConverter, GetAllProductsRequest>(src => src))
            .ForMember(dst => dst.Ordered, opt => opt.ConvertUsing<OrderedConverter, GetAllProductsRequest>(src => src))
            .ForMember(dst => dst.Paged, opt => opt.ConvertUsing<PagedConverter, GetAllProductsRequest>(src => src));

        CreateMap<Guid, GetProductQuery>()
            .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src));
    }
}