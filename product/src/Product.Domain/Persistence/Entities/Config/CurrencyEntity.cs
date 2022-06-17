using System;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Persistence.Entities
{
    public class CurrencyEntity : Entity, IAggregateRoot, ISoftDelete
    {
        public CurrencyEntity(string name, string code, string symbol)
        {
            Id = Guid.NewGuid();
            Name = name;
            Code = code;
            Symbol = symbol;
        }

        protected CurrencyEntity()
        {
        }

        public Guid Id { get; internal set; }
        public string Name { get; internal set; }
        public string Code { get; internal set; }
        public string Symbol { get; internal set; }
        public bool IsDeleted { get; internal set; }
    }
}