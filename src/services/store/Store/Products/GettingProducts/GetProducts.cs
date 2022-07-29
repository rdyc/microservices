using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Store.Products.GettingProducts;

public record GetProducts(
    PagedOption Option
) : IQuery<IListPaged<ProductShortInfo>>
{
    public static GetProducts Create(int? page, int? size) =>
        new(PagedOption.Create(page ?? 1, size ?? 10));
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
        var filter = Builders<ProductShortInfo>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, request.Option, cancellationToken);
    }
}