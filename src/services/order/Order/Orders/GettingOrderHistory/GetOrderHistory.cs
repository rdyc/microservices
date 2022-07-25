

using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Order.Orders.GettingOrderHistory;

public record GetOrderHistory(
    Guid OrderId,
    int PageNumber,
    int PageSize
) : IQuery<IListPaged<OrderHistory>>
{
    public static GetOrderHistory Create(Guid? cartId, int? pageNumber = 1, int? pageSize = 20)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        if (pageNumber is null or < 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize is null or < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        return new(cartId.Value, pageNumber.Value, pageSize.Value);
    }
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
        var (cartId, index, size) = request;

        var filter = Builders<OrderHistory>.Filter.Eq(e => e.AggregateId, cartId);

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}
