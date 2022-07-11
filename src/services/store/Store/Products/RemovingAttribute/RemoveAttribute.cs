using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Store.Products.RemovingAttribute;

public record RemoveAttribute(
    Guid ProductId, 
    ProductAttribute ProductAttribute
) : ICommand
{
    public static RemoveAttribute Create(Guid productId, ProductAttribute productAttribute)
    {
        if (productId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));

        if (productAttribute is null)
            throw new ArgumentNullException(nameof(productAttribute));

        return new (productId, productAttribute);
    }
}

internal class HandleRemoveAttribute : ICommandHandler<RemoveAttribute>
{
    private readonly IEventStoreDBRepository<Product> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRemoveAttribute(IEventStoreDBRepository<Product> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RemoveAttribute request, CancellationToken cancellationToken)
    {
        var (id, productAttribute) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                id,
                (product) => product.RemoveAttribute(productAttribute),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}