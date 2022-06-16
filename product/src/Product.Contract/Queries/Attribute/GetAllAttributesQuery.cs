using System.Collections.Generic;
using MediatR;
using Product.Contract.Dtos;
using Shared.Infrastructure.Request;

namespace Product.Contract.Queries
{
    public class GetAllAttributesQuery : IRequest<IEnumerable<IAttributeDto>>
    {
        public CriteriaQuery<AttributeField> Criteria { get; set; }
        public OrderedQuery<AttributeField> Ordered { get; set; }
        public PagedQuery Paged { get; set; }
    }
}