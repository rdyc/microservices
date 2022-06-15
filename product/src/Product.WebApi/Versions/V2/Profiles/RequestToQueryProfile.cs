using System;
using AutoMapper;
using Product.Contract.Queries;
using Product.WebApi.Versions.V2.Converters;
using Product.WebApi.Versions.V2.Models;

namespace Product.WebApi.Versions.V2.Profiles
{
    internal class RequestToQueryProfile : Profile
    {
        public RequestToQueryProfile() : base("V2 Request to query profile")
        {
            CreateMap<GetAllProductsRequest, GetAllProductsQuery>()
                .ForMember(dst => dst.Criteria, opt => opt.ConvertUsing<CriteriaConverter, GetAllProductsRequest>(src => src))
                .ForMember(dst => dst.Ordered, opt => opt.ConvertUsing<OrderedConverter, GetAllProductsRequest>(src => src))
                .ForMember(dst => dst.Paged, opt => opt.ConvertUsing<PagedConverter, GetAllProductsRequest>(src => src));
        }
    }
}