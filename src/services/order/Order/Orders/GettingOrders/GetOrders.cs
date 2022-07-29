using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Order.Orders.GettingOrders;

public record GetOrders(
    Guid ClientId,
    PagedOption Option
) : IQuery<IListPaged<OrderShortInfo>>
{
    public static GetOrders Create(Guid clientId, int? page, int? size) =>
        new(clientId, PagedOption.Create(page ?? 1, size ?? 10));
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
        var (clientId, option) = request;

        var filter = Builders<OrderShortInfo>.Filter.Eq(e => e.ClientId, clientId);

        return await collection.FindWithPagingAsync(filter, option, cancellationToken);
    }
}