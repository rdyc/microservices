using System;

namespace Shared.Infrastructure.Domain
{
    public interface ISoftDelete
    {
        DateTime? DeletedAt { get; }
    }
}