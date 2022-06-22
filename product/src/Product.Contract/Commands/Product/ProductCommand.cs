using System;
using MediatR;
using Product.Contract.Dtos;

namespace Product.Contract.Commands;

public class ProductCommand : IRequest<IProductDto>
{
    public Guid? Id { get; protected set; }
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public Guid CurrencyId { get; protected set; }
    public decimal Price { get; protected set; }
}