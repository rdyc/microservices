using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Store.Products.SellingProduct;

public record SellProduct(
    Guid ProductId,
    int Quantity
) : ICommand
{
    public static SellProduct Create(Guid productId, int quantity)
        => new(productId, quantity);
}

internal class HandleSellProduct : ICommandHandler<SellProduct>
{
    private readonly IEventStoreDBRepository<Product> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleSellProduct(
        IEventStoreDBRepository<Product> repository,
        IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(SellProduct request, CancellationToken cancellationToken)
    {
        var (productId, quantity) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                productId,
                (product) => product.Sold(quantity),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}