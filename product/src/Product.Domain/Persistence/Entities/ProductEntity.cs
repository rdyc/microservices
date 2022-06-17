using System;
using System.Collections.Generic;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Persistence.Entities
{
    internal class ProductEntity : Entity, IAggregateRoot, ISoftDelete
    {
        public ProductEntity(
            string name,
            string description,
            CurrencyReferenceEntity currency,
            decimal price)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            CurrencyRefId = currency.RefId;
            Currency = currency;
            Price = price;
        }

        protected ProductEntity()
        {
        }

        public Guid Id { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public Guid CurrencyRefId { get; internal set; }
        public CurrencyReferenceEntity Currency { get; internal set; }
        public decimal Price { get; internal set; }
        public ICollection<ProductAttributeEntity> Attributes { get; internal set; }
        public bool IsDeleted { get; internal set; }
    }
}