using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Shipment.Orders.GettingOrders;

public record GetOrders(
    PagedOption Option
) : IQuery<IListPaged<Order>>
{
    public static GetOrders Create(int? page, int? size) =>
        new(PagedOption.Create(page ?? 1, size ?? 10));
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
        var filter = Builders<Order>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, request.Option, cancellationToken);
    }
}