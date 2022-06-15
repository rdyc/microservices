using System;
using AutoMapper;
using Product.Contract.Queries;
using Product.WebApi.Versions.V1.Converters;
using Product.WebApi.Versions.V1.Models;

namespace Product.WebApi.Versions.V1.Profiles
{
    internal class RequestToQueryProfile : Profile
    {
        public RequestToQueryProfile() : base("V1 Request to query profile")
        {
            CreateMap<GetAllProductsRequest, GetAllProductsQuery>()
                .ForMember(dst => dst.Criteria, opt => opt.ConvertUsing<CriteriaConverter, GetAllProductsRequest>(src => src))
                .ForMember(dst => dst.Ordered, opt => opt.ConvertUsing<OrderedConverter, GetAllProductsRequest>(src => src))
                .ForMember(dst => dst.Paged, opt => opt.ConvertUsing<PagedConverter, GetAllProductsRequest>(src => src));
            
            CreateMap<Guid, GetProductQuery>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src));
        }
    }
}