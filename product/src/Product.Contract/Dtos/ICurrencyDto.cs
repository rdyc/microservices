using System;

namespace Product.Contract.Dtos
{
    public interface ICurrencyDto
    {
        Guid Id { get; }
        string Name { get; }
        string Code { get; }
        string Symbol { get; }
    }
}