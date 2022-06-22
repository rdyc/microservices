using System.Collections.Generic;
using MediatR;
using Product.Contract.Dtos;
using Shared.Infrastructure.Request;

namespace Product.Contract.Queries;

public class GetAllProductsQuery : IRequest<IEnumerable<IProductDto>>
{
    public CriteriaQuery<ProductField> Criteria { get; set; }
    public OrderedQuery<ProductField> Ordered { get; set; }
    public PagedQuery Paged { get; set; }
}