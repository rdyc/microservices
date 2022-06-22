using System.Collections.Generic;
using MediatR;
using Product.Contract.Dtos;
using Shared.Infrastructure.Request;

namespace Product.Contract.Queries;

public class GetListAttributesQuery : IRequest<IEnumerable<IAttributeDto>>
{
    public CriteriaQuery<AttributeField> Criteria { get; set; }
    public OrderedQuery<AttributeField> Ordered { get; set; }
}