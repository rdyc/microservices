using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Cart.ShoppingCarts.RemovingProduct;

public record RemoveProduct(
    Guid CartId,
    ShoppingCartProduct Product
): ICommand
{
    public static RemoveProduct Create(Guid? cartId, ShoppingCartProduct? product)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        if (product == null)
            throw new ArgumentOutOfRangeException(nameof(product));

        return new RemoveProduct(cartId.Value, product);
    }
}

internal class HandleRemoveProduct: ICommandHandler<RemoveProduct>
{
    private readonly IEventStoreDBRepository<ShoppingCart> cartRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRemoveProduct(
        IEventStoreDBRepository<ShoppingCart> cartRepository,
        IEventStoreDBAppendScope scope
    )
    {
        this.cartRepository = cartRepository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RemoveProduct command, CancellationToken cancellationToken)
    {
        var (cartId, pricedProduct) = command;

        await scope.Do((expectedRevision, eventMetadata) =>
            cartRepository.GetAndUpdate(
                cartId,
                cart => cart.RemoveProduct(pricedProduct),
                expectedRevision,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}