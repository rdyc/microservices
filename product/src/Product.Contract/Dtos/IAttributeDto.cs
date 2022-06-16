using System;
using Product.Contract.Enums;

namespace Product.Contract.Dtos
{
    public interface IAttributeDto
    {
        Guid Id { get; }
        string Name { get; }
        AttributeType Type { get; }
        string Unit { get; }
    }
}