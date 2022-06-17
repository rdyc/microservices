using System;
using Product.Contract.Enums;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Persistence.Entities
{
    internal class AttributeEntity : Entity, IAggregateRoot, ISoftDelete
    {
        public AttributeEntity(
            string name,
            AttributeType type,
            string unit)
        {
            Id = Guid.NewGuid();
            Name = name;
            Type = type;
            Unit = unit;
        }

        protected AttributeEntity()
        {
        }

        public Guid Id { get; internal set; }
        public string Name { get; internal set; }
        public AttributeType Type { get; internal set; }
        public string Unit { get; internal set; }
        public bool IsDeleted { get; internal set; }
    }
}