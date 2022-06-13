using System;
using MediatR;
using Product.Contract.Dto;

namespace Product.Contract.Query
{
    public class GetItemQuery : IRequest<IItemDto>
    {
        public Guid Id { get; set; }
    }
}