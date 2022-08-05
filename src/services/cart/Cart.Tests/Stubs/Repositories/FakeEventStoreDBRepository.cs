using FW.Core.Aggregates;
using FW.Core.EventStoreDB.Repository;
using FW.Core.Tracing;

namespace Cart.Tests.Stubs.Repositories;

public class FakeEventStoreDBRepository<T> : IEventStoreDBRepository<T> where T : class, IAggregate
{
    public Dictionary<Guid, T> Aggregates { get; private set; }

    public FakeEventStoreDBRepository(params T[] aggregates)
    {
        Aggregates = aggregates.ToDictionary(ks=> ks.Id, vs => vs);
    }

    public Task<T?> Find(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Aggregates.GetValueOrDefault(id));
    }

    public async Task<ulong> Add(T aggregate, TraceMetadata? traceMetadata = null, CancellationToken cancellationToken = default)
    {
        Aggregates.Add(aggregate.Id, aggregate);
        return await Task.FromResult(Convert.ToUInt64(aggregate.Version));
    }

    public async Task<ulong> Update(T aggregate, ulong? expectedVersion = null, TraceMetadata? traceMetadata = null, CancellationToken cancellationToken = default)
    {
        Aggregates[aggregate.Id] = aggregate;
        return await Task.FromResult(Convert.ToUInt64(aggregate.Version));
    }

    public async Task<ulong> Delete(T aggregate, ulong? expectedVersion = null, TraceMetadata? traceMetadata = null, CancellationToken cancellationToken = default)
    {
        Aggregates.Remove(aggregate.Id);
        return await Task.FromResult(Convert.ToUInt64(aggregate.Version));
    }
}
