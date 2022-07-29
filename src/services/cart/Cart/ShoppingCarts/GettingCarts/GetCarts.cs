using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Cart.ShoppingCarts.GettingCarts;

public record GetCarts(
    PagedOption Option
) : IQuery<IListPaged<ShoppingCartShortInfo>>
{
    public static GetCarts Create(int? page, int? size) =>
        new(PagedOption.Create(page ?? 1, size ?? 10));
}

internal class HandleGetCarts : IQueryHandler<GetCarts, IListPaged<ShoppingCartShortInfo>>
{
    private readonly IMongoCollection<ShoppingCartShortInfo> collection;

    public HandleGetCarts(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<ShoppingCartShortInfo>();
        this.collection = database.GetCollection<ShoppingCartShortInfo>(collectionName);
    }

    public async Task<IListPaged<ShoppingCartShortInfo>> Handle(GetCarts request, CancellationToken cancellationToken)
    {
        var filter = Builders<ShoppingCartShortInfo>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, request.Option, cancellationToken);
    }
}