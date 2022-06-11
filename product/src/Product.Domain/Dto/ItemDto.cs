using System;
using Product.Contract.Dto;

namespace Product.Domain.Dto
{
    internal class ItemDto : IItemDto
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
    }
}