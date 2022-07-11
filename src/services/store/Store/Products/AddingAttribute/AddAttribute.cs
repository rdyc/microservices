using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Store.Products.AddingAttribute;

public record AddAttribute(
    Guid ProductId, 
    Guid AttributeId,
    string Value
) : ICommand
{
    public static AddAttribute Create(Guid productId, Guid attributeId, string value)
    {
        if (productId == Guid.Empty)
            throw new ArgumentNullException(nameof(productId));

        if (attributeId == Guid.Empty)
            throw new ArgumentNullException(nameof(attributeId));

        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        return new (productId, attributeId, value);
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
        var (productId, attributeId, value) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                productId,
                (product) => product.AddAttribute(attributeId, value),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}