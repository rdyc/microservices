using AutoMapper;
using Product.Contract.Queries;
using Product.WebApi.Versions.V2.Models;
using Shared.Infrastructure.Request;

namespace Product.WebApi.Versions.V2.Converters
{
    internal class OrderedConverter : IValueConverter<GetAllProductsRequest, OrderedQuery<ProductField>>
    {
        public OrderedQuery<ProductField> Convert(GetAllProductsRequest sourceMember, ResolutionContext context)
        {
            return null;
        }
    }
}