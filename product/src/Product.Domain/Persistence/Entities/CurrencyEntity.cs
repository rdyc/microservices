using System;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Persistence.Entities
{
    public class CurrencyEntity : BaseEntity, IAggregateRoot, ISoftDelete
    {
        public CurrencyEntity(string name, string symbol)
        {
            Id = Guid.NewGuid();
            Name = name;
            Symbol = symbol;
            IsTransient = true;
        }

        public Guid Id { get; internal set; }
        public string Name { get; internal set; }
        public string Symbol { get; internal set; }
        public DateTime? DeletedAt { get; internal set;}
    }
}