using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;
using Store.Currencies;

namespace Store.Products.UpdatingPrice;

public record UpdatePrice(
    Guid Id,
    Currency Currency,
    decimal Price
) : ICommand;

internal class HandleUpdatePrice : ICommandHandler<UpdatePrice>
{
    private readonly IEventStoreDBRepository<Product> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleUpdatePrice(IEventStoreDBRepository<Product> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(UpdatePrice request, CancellationToken cancellationToken)
    {
        var (id, currency, price) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                id,
                (product) => product.UpdatePrice(currency,price),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}