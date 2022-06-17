using System.Collections.Generic;

namespace Shared.Infrastructure.Domain
{
    public interface IEntity
    {
        ICollection<IDomainEvent> Events { get; }
    }
}