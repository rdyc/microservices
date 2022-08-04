using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.Tracing;

namespace Cart.Tests.Stubs.Events;

public class DummyEventStoreAppendScope : IEventStoreDBAppendScope
{
    private readonly ulong? expectedVersion;
    private readonly TraceMetadata? traceMetadata;

    public DummyEventStoreAppendScope(
        ulong? expectedVersion = null,
        TraceMetadata? traceMetadata = null
    )
    {
        this.expectedVersion = expectedVersion;
        this.traceMetadata = traceMetadata;
    }

    public async Task Do(Func<ulong?, TraceMetadata?, Task<ulong>> handler)
    {
        await handler(expectedVersion, traceMetadata);
    }
}
