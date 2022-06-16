using System;
using MediatR;
using Product.Contract.Dtos;

namespace Product.Contract.Queries
{
    public class GetAttributeQuery : IRequest<IAttributeDto>
    {
        public Guid Id { get; set; }
    }
}