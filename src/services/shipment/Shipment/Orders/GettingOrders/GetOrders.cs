using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Shipment.Orders.GettingOrders;

public record GetOrders(
    int Index,
    int Size
) : IQuery<IListPaged<Order>>
{
    public static GetOrders Create(int? index = 0, int? size = 10)
    {
        if (index is null or < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (size is null or < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(size));

        return new(index.Value, size.Value);
    }
}

internal class HandleGetOrders : IQueryHandler<GetOrders, IListPaged<Order>>
{
    private readonly IMongoCollection<Order> collection;

    public HandleGetOrders(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<Order>();
        this.collection = database.GetCollection<Order>(collectionName);
    }

    public async Task<IListPaged<Order>> Handle(GetOrders request, CancellationToken cancellationToken)
    {
        var (index, size) = request;

        var filter = Builders<Order>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}