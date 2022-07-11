using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Store.Products.AddingAttribute;

public record AddAttribute(
    Guid ProductId, 
    ProductAttribute ProductAttribute
) : ICommand
{
    public static AddAttribute Create(Guid productId, ProductAttribute productAttribute)
    {
        if (productId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));

        if (productAttribute is null)
            throw new ArgumentNullException(nameof(productAttribute));

        return new (productId, productAttribute);
    }
}

internal class HandleAddAttribute : ICommandHandler<AddAttribute>
{
    private readonly IEventStoreDBRepository<Product> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleAddAttribute(IEventStoreDBRepository<Product> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(AddAttribute request, CancellationToken cancellationToken)
    {
        var (id, productAttribute) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                id,
                (product) => product.AddAttribute(productAttribute),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}