using EventStore.Client;
using FW.Core.EventStoreDB.Events;
using FW.Core.Exceptions;
using FW.Core.Queries;

namespace Cart.ShoppingCarts.GettingCartAtVersion;

public record GetCartAtVersion(
    Guid CartId,
    ulong Version
) : IQuery<ShoppingCart>
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

internal class HandleGetCartAtVersion : IQueryHandler<GetCartAtVersion, ShoppingCart>
{
    private readonly EventStoreClient eventStore;

    public HandleGetCartAtVersion(EventStoreClient eventStore)
    {
        this.eventStore = eventStore;
    }

    public async Task<ShoppingCart> Handle(GetCartAtVersion request, CancellationToken cancellationToken)
    {
        var (cartId, version) = request;

        var cart = await eventStore.AggregateStream<ShoppingCart>(
            cartId,
            cancellationToken,
            version
        );

        if (cart == null)
            throw AggregateNotFoundException.For<ShoppingCart>(request.CartId);

        return cart;
    }
}