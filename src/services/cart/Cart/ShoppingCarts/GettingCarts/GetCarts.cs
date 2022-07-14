using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Cart.ShoppingCarts.GettingCarts;

public record GetCarts(
    int PageNumber,
    int PageSize
) : IQuery<IListPaged<ShoppingCartShortInfo>>
{
    public static GetCarts Create(int? pageNumber = 1, int? pageSize = 20)
    {
        if (pageNumber is null or <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (pageSize is null or <= 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        return new GetCarts(pageNumber.Value, pageSize.Value);
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
