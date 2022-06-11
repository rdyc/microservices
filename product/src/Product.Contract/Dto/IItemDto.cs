using System;

namespace Product.Contract.Dto
{
    public interface IItemDto
    {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }
    }
}