using System;
using Product.Contract.Dtos;

namespace Product.Domain.Dtos
{
    internal class ProductDto : IProductDto
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
    }
}