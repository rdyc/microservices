using System;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Persistence.Entities
{
    internal class ProductAttributeEntity : BaseEntity
    {
        public ProductAttributeEntity(
            ProductEntity product,
            int sequence,
            string name,
            AttributeType type,
            string value,
            string unit)
        {
            Id = Guid.NewGuid();
            ProductId = product.Id;
            Product = product;
            Sequence = sequence;
            Name = name;
            Type = type;
            Value = value;
            Unit = unit;
            IsTransient = true;
        }

        protected ProductAttributeEntity()
        {
        }

        public Guid Id { get; internal set; }
        public Guid ProductId { get; internal set; }
        public ProductEntity Product { get; internal set; }
        public int Sequence { get; internal set; }
        public string Name { get; internal set; }
        public AttributeType Type { get; internal set; }
        public string Value { get; internal set; }
        public string Unit { get; internal set; }
    }
}