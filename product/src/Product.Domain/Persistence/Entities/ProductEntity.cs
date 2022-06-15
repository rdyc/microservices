using System;
using System.Collections.Generic;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Persistence.Entities
{
    internal class ProductEntity : BaseEntity, IAggregateRoot, ISoftDelete
    {
        public ProductEntity(
            string name,
            string description)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            IsTransient = true;
        }

        protected ProductEntity()
        {
        }

        public Guid Id { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public decimal Price { get; internal set; }
        public ProductCurrencyEntity Currency { get; internal set; }
        public ICollection<ProductAttributeEntity> Attributes { get; internal set; }
        public DateTime? DeletedAt { get; internal set; }
    }
}