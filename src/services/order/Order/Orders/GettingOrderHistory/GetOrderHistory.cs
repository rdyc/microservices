using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Order.Orders.GettingOrderHistory;

public record GetOrderHistory(
    Guid OrderId,
    PagedOption Option
) : IQuery<IListPaged<OrderHistory>>
{
    public static GetOrderHistory Create(Guid orderId, int? page, int? size) =>
        new(orderId, PagedOption.Create(page ?? 1, size ?? 10));
}

internal class HandleGetOrderHistory : IQueryHandler<GetOrderHistory, IListPaged<OrderHistory>>
{
    private readonly IMongoCollection<OrderHistory> collection;

    public HandleGetOrderHistory(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<OrderHistory>();
        collection = database.GetCollection<OrderHistory>(collectionName);
    }

    public async Task<IListPaged<OrderHistory>> Handle(GetOrderHistory request, CancellationToken cancellationToken)
    {
        var (orderId, option) = request;

        var filter = Builders<OrderHistory>.Filter.Eq(e => e.AggregateId, orderId);

        return await collection.FindWithPagingAsync(filter, option, cancellationToken);
    }
}
