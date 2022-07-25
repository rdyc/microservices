using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;
using Order.ShoppingCarts.FinalizingCart;

namespace Order.Orders.InitializingOrder;

public record InitializeOrder(
    Guid OrderId,
    Guid ClientId,
    IEnumerable<ShoppingCartProduct> Products,
    decimal TotalPrice
) : ICommand
{
    public static InitializeOrder Create(
        Guid? orderId,
        Guid? clientId,
        IEnumerable<ShoppingCartProduct>? products,
        decimal? totalPrice
    )
    {
        if (!orderId.HasValue)
            throw new ArgumentNullException(nameof(orderId));
        if (!clientId.HasValue)
            throw new ArgumentNullException(nameof(clientId));
        if (products == null)
            throw new ArgumentNullException(nameof(products));
        if (!totalPrice.HasValue)
            throw new ArgumentNullException(nameof(totalPrice));

        return new(orderId.Value, clientId.Value, products, totalPrice.Value);
    }
}

public class HandleInitializeOrder : ICommandHandler<InitializeOrder>
{
    private readonly IEventStoreDBRepository<Order> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleInitializeOrder(
        IEventStoreDBRepository<Order> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(InitializeOrder command, CancellationToken cancellationToken)
    {
        var (orderId, clientId, products, totalPrice) = command;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                Order.Initialize(orderId, clientId, products, totalPrice),
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}
