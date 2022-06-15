using System;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Persistence.Entities
{
    internal class AttributeEntity : BaseEntity, IAggregateRoot, ISoftDelete
    {
        public AttributeEntity(
            string name,
            AttributeType type,
            string value,
            string unit)
        {
            Id = Guid.NewGuid();
            Name = name;
            Type = type;
            Value = value;
            Unit = unit;
            IsTransient = true;
        }

        protected AttributeEntity()
        {
        }

        public Guid Id { get; internal set; }
        public string Name { get; internal set; }
        public AttributeType Type { get; internal set; }
        public string Value { get; internal set; }
        public string Unit { get; internal set; }
        public DateTime? DeletedAt { get; internal set; }
    }
}