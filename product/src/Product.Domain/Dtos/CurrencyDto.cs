using System;
using Product.Contract.Dtos;

namespace Product.Domain.Dtos;

internal class CurrencyDto : ICurrencyDto
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }
    public string Symbol { get; private set; }
}