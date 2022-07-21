using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Cart.ShoppingCarts.GettingCarts;

public record GetCarts(
    int Index,
    int Size
) : IQuery<IListPaged<ShoppingCartShortInfo>>
{
    public static GetCarts Create(int? index = 0, int? size = 10)
    {
        if (index is null or < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (size is null or < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(size));

        return new(index.Value, size.Value);
    }
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
        var (index, size) = request;

        var filter = Builders<ShoppingCartShortInfo>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}
