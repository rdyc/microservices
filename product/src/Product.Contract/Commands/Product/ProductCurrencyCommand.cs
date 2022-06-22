using System;

namespace Product.Contract.Commands;

public class ProductCurrencyCommand
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; }
    public string Code { get; protected set; }
    public string Symbol { get; protected set; }
}