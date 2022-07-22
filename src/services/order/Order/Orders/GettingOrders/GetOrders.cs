using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Order.Orders.GettingOrders;

public record GetOrders(
    Guid ClientId,
    int Index,
    int Size
) : IQuery<IListPaged<OrderShortInfo>>
{
    public static GetOrders Create(Guid clientId, int? index = 0, int? size = 10)
    {
        if (index is null or < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (size is null or < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(size));

        return new(clientId, index.Value, size.Value);
    }
}

internal class HandleGetOrders : IQueryHandler<GetOrders, IListPaged<OrderShortInfo>>
{
    private readonly IMongoCollection<OrderShortInfo> collection;

    public HandleGetOrders(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<OrderShortInfo>();
        this.collection = database.GetCollection<OrderShortInfo>(collectionName);
    }

    public async Task<IListPaged<OrderShortInfo>> Handle(GetOrders request, CancellationToken cancellationToken)
    {
        var (clientId, index, size) = request;

        var filter = Builders<OrderShortInfo>.Filter.Eq(e => e.ClientId, clientId);

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}