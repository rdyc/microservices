using System;

namespace Product.Contract.Dtos
{
    public interface IProductDto
    {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }
        ICurrencyDto Currency { get; }
        decimal Price { get; }
    }
}