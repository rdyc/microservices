using System.Collections.Generic;
using MediatR;
using Product.Contract.Dto;

namespace Product.Contract.Query
{
    public class GetAllItemsQuery : IRequest<IEnumerable<IItemDto>>
    {
         
    }
}