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
    public static GetOrderAtVersion Create(Guid? cartId, ulong? version)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        if (version == null)
            throw new ArgumentOutOfRangeException(nameof(version));

        return new(cartId.Value, version.Value);
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
        var (cartId, version) = request;

        var cart = await eventStore.AggregateStream<Order>(
            cartId,
            cancellationToken,
            version
        );

        if (cart == null)
            throw AggregateNotFoundException.For<Order>(request.OrderId);

        return cart;
    }
}