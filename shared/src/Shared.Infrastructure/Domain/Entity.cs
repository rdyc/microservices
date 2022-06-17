using System.Collections.Generic;

namespace Shared.Infrastructure.Domain
{
    public class Entity : IEntity
    {
        public Entity()
        {
            Events = new List<IDomainEvent>();
        }

        public ICollection<IDomainEvent> Events { get; set; }
    }
}