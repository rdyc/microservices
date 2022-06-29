using System;
using Product.Domain.Persistence.Entities;
using Shared.Infrastructure.Domain;

namespace Product.Domain.Events;

internal class ProductCreatedEvent : IDomainEvent
{
    public ProductCreatedEvent(ProductEntity message)
    {
        AggregateId = message.Id;
        Time = DateTime.UtcNow;
        Message = message;
    }

    public Guid AggregateId { get; private set; }
    public DateTime Time { get; private set; }
    public ProductEntity Message { get; private set; }
}