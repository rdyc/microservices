

using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Cart.ShoppingCarts.GettingCartHistory;

public record GetCartHistory(
    Guid CartId,
    PagedOption Option
) : IQuery<IListPaged<ShoppingCartHistory>>
{
    public static GetCartHistory Create(Guid cartId, int? page, int? size) =>
        new(cartId, PagedOption.Create(page ?? 1, size ?? 10));
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
        var (cartId, option) = request;

        var filter = Builders<ShoppingCartHistory>.Filter.Eq(e => e.AggregateId, cartId);

        return await collection.FindWithPagingAsync(filter, option, cancellationToken);
    }
}
