using System.Collections.Generic;
using MediatR;
using Product.Contract.Dtos;
using Shared.Infrastructure.Request;

namespace Product.Contract.Queries;

public class GetAllCurrenciesQuery : IRequest<IEnumerable<ICurrencyDto>>
{
    public CriteriaQuery<CurrencyField> Criteria { get; set; }
    public OrderedQuery<CurrencyField> Ordered { get; set; }
    public PagedQuery Paged { get; set; }
}