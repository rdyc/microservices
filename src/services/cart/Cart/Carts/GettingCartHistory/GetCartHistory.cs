

using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Cart.Carts.GettingCartHistory;

public record GetCartHistory(
    Guid CartId,
    PagedOption Option
) : IQuery<IListPaged<CartHistory>>
{
    public static GetCartHistory Create(Guid cartId, int? page, int? size) =>
        new(cartId, PagedOption.Create(page ?? 1, size ?? 10));
}

internal class HandleGetCartHistory : IQueryHandler<GetCartHistory, IListPaged<CartHistory>>
{
    private readonly IMongoCollection<CartHistory> collection;

    public HandleGetCartHistory(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<CartHistory>();
        collection = database.GetCollection<CartHistory>(collectionName);
    }

    public async Task<IListPaged<CartHistory>> Handle(GetCartHistory request, CancellationToken cancellationToken)
    {
        var (cartId, option) = request;

        var filter = Builders<CartHistory>.Filter.Eq(e => e.AggregateId, cartId);

        return await collection.FindWithPagingAsync(filter, option, cancellationToken);
    }
}
