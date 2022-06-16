using System;
using System.Collections.Generic;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Persistence.Entities
{
    internal class CurrencyReferenceEntity : BaseEntity, IAggregateRoot
    {
        public CurrencyReferenceEntity(Guid id, string name, string code, string symbol)
        {
            RefId = Guid.NewGuid();
            Id = id;
            Name = name;
            Code = code;
            Symbol = symbol;
        }

        protected CurrencyReferenceEntity()
        {
        }

        public Guid RefId { get; internal set; }
        public Guid Id { get; internal set; }
        public string Name { get; internal set; }
        public string Code { get; internal set; }
        public string Symbol { get; internal set; }
        public ICollection<ProductEntity> Products { get; internal set; }
    }
}