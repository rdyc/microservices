using EventStore.Client;
using FW.Core.EventStoreDB.Events;
using FW.Core.Exceptions;
using FW.Core.Queries;

namespace Cart.Carts.GettingCartAtVersion;

public record GetCartAtVersion(
    Guid CartId,
    ulong Version
) : IQuery<Cart>
{
    public static GetCartAtVersion Create(Guid? cartId, ulong? version)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        if (version == null)
            throw new ArgumentOutOfRangeException(nameof(version));

        return new(cartId.Value, version.Value);
    }
}

internal class HandleGetCartAtVersion : IQueryHandler<GetCartAtVersion, Cart>
{
    private readonly EventStoreClient eventStore;

    public HandleGetCartAtVersion(EventStoreClient eventStore)
    {
        this.eventStore = eventStore;
    }

    public async Task<Cart> Handle(GetCartAtVersion request, CancellationToken cancellationToken)
    {
        var (cartId, version) = request;

        var cart = await eventStore.AggregateStream<Cart>(
            cartId,
            cancellationToken,
            version
        );

        if (cart == null)
            throw AggregateNotFoundException.For<Cart>(request.CartId);

        return cart;
    }
}