using System;
using MediatR;
using Product.Contract.Dtos;

namespace Product.Contract.Queries;

public class GetProductQuery : IRequest<IProductDto>
{
    public Guid Id { get; set; }
}