using System;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Persistence.Entities
{
    internal class ProductAttributeEntity : Entity
    {
        public ProductAttributeEntity(
            ProductEntity product,
            int sequence,
            AttributeReferenceEntity attribute,
            string value)
        {
            Id = Guid.NewGuid();
            ProductId = product.Id;
            Product = product;
            Sequence = sequence;
            AttributeRefId = attribute.RefId;
            Attribute = attribute;
            Value = value;
        }

        protected ProductAttributeEntity()
        {
        }

        public Guid Id { get; internal set; }
        public Guid ProductId { get; internal set; }
        public ProductEntity Product { get; internal set; }
        public int Sequence { get; internal set; }
        public Guid AttributeRefId { get; internal set; }
        public AttributeReferenceEntity Attribute { get; internal set; }
        public string Value { get; internal set; }
    }
}