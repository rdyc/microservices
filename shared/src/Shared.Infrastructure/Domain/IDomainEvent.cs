using System;

namespace Shared.Infrastructure.Domain
{
    public interface IDomainEvent
    {
        Guid AggregateId { get; }
        DateTime Time { get; }
    }
}