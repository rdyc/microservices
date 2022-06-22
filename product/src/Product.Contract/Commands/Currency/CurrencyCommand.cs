using System;
using MediatR;
using Product.Contract.Dtos;

namespace Product.Contract.Commands;

public class CurrencyCommand : IRequest<ICurrencyDto>
{
    public Guid? Id { get; protected set; }
    public string Name { get; protected set; }
    public string Code { get; protected set; }
    public string Symbol { get; protected set; }
}