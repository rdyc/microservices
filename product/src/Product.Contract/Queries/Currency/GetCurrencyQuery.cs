using System;
using MediatR;
using Product.Contract.Dtos;

namespace Product.Contract.Queries;

public class GetCurrencyQuery : IRequest<ICurrencyDto>
{
    public Guid Id { get; set; }
}