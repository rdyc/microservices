using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Store.Products.RemovingAttribute;

public record RemoveAttribute(
    Guid ProductId, 
    Guid AttributeId
) : ICommand
{
    public static RemoveAttribute Create(Guid productId, Guid attributeId)
    {
        if (productId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));

        if (attributeId == Guid.Empty)
            throw new ArgumentNullException(nameof(attributeId));

        return new (productId, attributeId);
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
        var (productId, attributeId) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                productId,
                (product) => product.RemoveAttribute(attributeId),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}