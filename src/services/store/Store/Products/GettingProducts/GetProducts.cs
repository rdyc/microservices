using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Store.Products.GettingProducts;

public record GetProducts(
    int PageNumber,
    int PageSize
) : IQuery<IListPaged<ProductShortInfo>>
{
    public static GetProducts Create(int? pageNumber = 1, int? pageSize = 20)
    {
        if (pageNumber is null or <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize is null or <= 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        return new GetProducts(pageNumber.Value, pageSize.Value);
    }
}

internal class HandleGetProducts : IQueryHandler<GetProducts, IListPaged<ProductShortInfo>>
{
    private readonly IMongoCollection<ProductShortInfo> collection;

    public HandleGetProducts(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<ProductShortInfo>();
        collection = database.GetCollection<ProductShortInfo>(collectionName);
    }

    public async Task<IListPaged<ProductShortInfo>> Handle(GetProducts request, CancellationToken cancellationToken)
    {
        var (index, size) = request;

        var filter = Builders<ProductShortInfo>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}