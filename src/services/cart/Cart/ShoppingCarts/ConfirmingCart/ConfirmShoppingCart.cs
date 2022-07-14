using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Cart.ShoppingCarts.ConfirmingCart;

public record ConfirmShoppingCart(
    Guid CartId
) : ICommand
{
    public static ConfirmShoppingCart Create(Guid? cartId)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        return new ConfirmShoppingCart(cartId.Value);
    }
}

internal class HandleConfirmCart : ICommandHandler<ConfirmShoppingCart>
{
    private readonly IEventStoreDBRepository<ShoppingCart> cartRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleConfirmCart(
        IEventStoreDBRepository<ShoppingCart> cartRepository,
        IEventStoreDBAppendScope scope
    )
    {
        this.cartRepository = cartRepository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(ConfirmShoppingCart command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedRevision, eventMetadata) =>
            cartRepository.GetAndUpdate(
                command.CartId,
                cart => cart.Confirm(),
                expectedRevision,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}