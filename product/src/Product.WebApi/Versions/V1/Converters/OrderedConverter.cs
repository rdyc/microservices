using AutoMapper;
using Product.Contract.Query;
using Product.WebApi.Versions.V1.Models;
using Shared.Infrastructure.Request;

namespace Product.WebApi.Versions.V1.Converters
{
    internal class OrderedConverter : IValueConverter<GetAllItemsRequest, OrderedQuery<ItemField>>
    {
        public OrderedQuery<ItemField> Convert(GetAllItemsRequest sourceMember, ResolutionContext context)
        {
            // if (sourceMember.Index.HasValue && sourceMember.Size.HasValue)
            // {
            //     return new PagedQuery(sourceMember.Index.Value, sourceMember.Size.Value);
            // }

            return null;
        }
    }
}