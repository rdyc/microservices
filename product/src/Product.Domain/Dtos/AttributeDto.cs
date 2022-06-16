using System;
using Product.Contract.Dtos;
using Product.Contract.Enums;

namespace Product.Domain.Dtos
{
    internal class AttributeDto : IAttributeDto
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public AttributeType Type { get; private set; }
        public string Value { get; private set; }
        public string Unit { get; private set; }
    }
}