using EventStore.Client;
using FW.Core.EventStoreDB.Events;
using FW.Core.Exceptions;
using FW.Core.Queries;

namespace Order.Orders.GettingOrderAtVersion;

public record GetOrderAtVersion(
    Guid OrderId,
    ulong Version
) : IQuery<Order>
{
    public static GetOrderAtVersion Create(Guid? orderId, ulong? version)
    {
        if (orderId == null || orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));

        if (version == null)
            throw new ArgumentOutOfRangeException(nameof(version));

        return new(orderId.Value, version.Value);
    }
}

internal class HandleGetOrderAtVersion : IQueryHandler<GetOrderAtVersion, Order>
{
    private readonly EventStoreClient eventStore;

    public HandleGetOrderAtVersion(EventStoreClient eventStore)
    {
        this.eventStore = eventStore;
    }

    public async Task<Order> Handle(GetOrderAtVersion request, CancellationToken cancellationToken)
    {
        var (orderId, version) = request;

        var order = await eventStore.AggregateStream<Order>(
            orderId,
            cancellationToken,
            version
        );

        if (order == null)
            throw AggregateNotFoundException.For<Order>(request.OrderId);

        return order;
    }
}