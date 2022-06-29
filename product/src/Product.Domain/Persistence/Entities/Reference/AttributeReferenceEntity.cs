using System;
using System.Collections.Generic;
using Product.Contract.Enums;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Persistence.Entities;

internal class AttributeReferenceEntity : Entity, IAggregateRoot
{
    public AttributeReferenceEntity(Guid id, string name, AttributeType type, string unit)
    {
        RefId = Guid.NewGuid();
        Id = id;
        Name = name;
        Type = type;
        Unit = unit;
    }

    protected AttributeReferenceEntity()
    {
    }

    public Guid RefId { get; internal set; }
    public Guid Id { get; internal set; }
    public string Name { get; internal set; }
    public AttributeType Type { get; internal set; }
    public string Unit { get; internal set; }
    public ICollection<ProductAttributeEntity> ProductAttributes { get; internal set; }
}