using EventStore.Client;
using FW.Core.EventStoreDB.Events;
using FW.Core.Exceptions;
using FW.Core.Queries;

namespace Store.Products.GettingProductAtVersion;

public record GetProductAtVersion(
    Guid ProductId,
    ulong Version
) : IQuery<Product>
{
    public static GetProductAtVersion Create(Guid? productId, ulong? version)
    {
        if (productId == null || productId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));

        if (version is null or < 0)
            throw new ArgumentOutOfRangeException(nameof(version));

        return new GetProductAtVersion(productId.Value, version.Value);
    }
}

internal class HandleGetProductAtVersion : IQueryHandler<GetProductAtVersion, Product>
{
    private readonly EventStoreClient eventStore;

    public HandleGetProductAtVersion(EventStoreClient eventStore)
    {
        this.eventStore = eventStore;
    }

    public async Task<Product> Handle(GetProductAtVersion request, CancellationToken cancellationToken)
    {
        var product = await eventStore.AggregateStream<Product>(
            request.ProductId,
            cancellationToken,
            request.Version
        );

        if (product == null)
            throw AggregateNotFoundException.For<Product>(request.ProductId);

        return product;
    }
}