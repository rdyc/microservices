using AutoMapper;
using Product.Contract.Query;
using Product.WebApi.Converters;
using Product.WebApi.Model;
using Shared.Infrastructure.Request;

namespace Product.WebApi
{
    public class RequestToQueryProfile : Profile
    {
        public RequestToQueryProfile() : base("Request to query profile")
        {
            CreateMap<GetAllItemsRequest, GetAllItemsQuery>()
                .ForMember(dst => dst.Criteria, opt => opt.ConvertUsing<CriteriaConverter, GetAllItemsRequest>(src => src))
                // .ForMember(dst => dst.Criteria, opt => opt.Ignore())
                .ForMember(dst => dst.Ordered, opt => opt.ConvertUsing<OrderedConverter, GetAllItemsRequest>(src => src))
                // .ForMember(dst => dst.Ordered, opt => opt.Ignore())
                .ForMember(dst => dst.Paged, opt => opt.ConvertUsing<PagedConverter, GetAllItemsRequest>(src => src));
        }
    }
}