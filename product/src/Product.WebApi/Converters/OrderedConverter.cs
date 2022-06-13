using AutoMapper;
using Product.Contract.Query;
using Product.WebApi.Model;
using Shared.Infrastructure.Request;

namespace Product.WebApi.Converters
{
    public class OrderedConverter : IValueConverter<GetAllItemsRequest, OrderedQuery<ItemField>>
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