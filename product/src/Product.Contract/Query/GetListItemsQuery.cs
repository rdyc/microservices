using System.Collections.Generic;
using MediatR;
using Product.Contract.Dto;
using Shared.Infrastructure.Request;

namespace Product.Contract.Query
{
    public class GetListItemsQuery : IRequest<IEnumerable<IItemDto>>
    {
        public CriteriaQuery<ItemField> Criteria { get; set; }
        public OrderedQuery<ItemField> Ordered { get; set; }
    }
}