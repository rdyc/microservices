using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Cart.Carts.GettingCarts;

public record GetCarts(
    PagedOption Option
) : IQuery<IListPaged<CartShortInfo>>
{
    public static GetCarts Create(int? page, int? size) =>
        new(PagedOption.Create(page ?? 1, size ?? 10));
}

internal class HandleGetCarts : IQueryHandler<GetCarts, IListPaged<CartShortInfo>>
{
    private readonly IMongoCollection<CartShortInfo> collection;

    public HandleGetCarts(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<CartShortInfo>();
        this.collection = database.GetCollection<CartShortInfo>(collectionName);
    }

    public async Task<IListPaged<CartShortInfo>> Handle(GetCarts request, CancellationToken cancellationToken)
    {
        var filter = Builders<CartShortInfo>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, request.Option, cancellationToken);
    }
}