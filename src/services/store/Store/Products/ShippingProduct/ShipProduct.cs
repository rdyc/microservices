using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Store.Products.ShippingProduct;

public record ShipProduct(
    Guid ProductId,
    int Quantity
) : ICommand
{
    public static ShipProduct Create(Guid productId, int quantity)
        => new(productId, quantity);
}

internal class HandleShipProduct : ICommandHandler<ShipProduct>
{
    private readonly IEventStoreDBRepository<Product> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleShipProduct(
        IEventStoreDBRepository<Product> repository,
        IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(ShipProduct request, CancellationToken cancellationToken)
    {
        var (productId, quantity) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                productId,
                (product) => product.PullStock(quantity),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}