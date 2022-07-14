using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Cart.ShoppingCarts.AddingProduct;

public record AddProduct(
    Guid CartId,
    ShoppingCartProduct Product
) : ICommand
{
    public static AddProduct Create(Guid? cartId, ShoppingCartProduct? product)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        if (product == null)
            throw new ArgumentOutOfRangeException(nameof(product));

        return new AddProduct(cartId.Value, product);
    }
}

internal class HandleAddProduct : ICommandHandler<AddProduct>
{
    private readonly IEventStoreDBRepository<ShoppingCart> cartRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleAddProduct(
        IEventStoreDBRepository<ShoppingCart> cartRepository,
        IEventStoreDBAppendScope scope
    )
    {
        this.cartRepository = cartRepository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(AddProduct command, CancellationToken cancellationToken)
    {
        var (cartId, product) = command;

        await scope.Do((expectedRevision, eventMetadata) =>
            cartRepository.GetAndUpdate(
                cartId,
                cart => cart.AddProduct(product),
                expectedRevision,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}