

using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Cart.ShoppingCarts.GettingCartHistory;

public record GetCartHistory(
    Guid CartId,
    int PageNumber,
    int PageSize
) : IQuery<IListPaged<ShoppingCartHistory>>
{
    public static GetCartHistory Create(Guid? cartId, int? pageNumber = 1, int? pageSize = 20)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        if (pageNumber is null or < 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize is null or < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        return new GetCartHistory(cartId.Value, pageNumber.Value, pageSize.Value);
    }
}

internal class HandleGetCartHistory : IQueryHandler<GetCartHistory, IListPaged<ShoppingCartHistory>>
{
    private readonly IMongoCollection<ShoppingCartHistory> collection;

    public HandleGetCartHistory(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<ShoppingCartHistory>();
        collection = database.GetCollection<ShoppingCartHistory>(collectionName);
    }

    public async Task<IListPaged<ShoppingCartHistory>> Handle(GetCartHistory request, CancellationToken cancellationToken)
    {
        var (cartId, index, size) = request;

        var filter = Builders<ShoppingCartHistory>.Filter.Eq(e => e.AggregateId, cartId);

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}
