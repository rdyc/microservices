using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Store.Products.GettingProductHistory;

public record GetProductHistory(
    Guid ProductId,
    PagedOption Option
) : IQuery<IListPaged<ProductHistory>>
{
    public static GetProductHistory Create(Guid productId, int? page, int? size) =>
        new(productId, PagedOption.Create(page ?? 1, size ?? 10));
};

internal class HandleGetProductHistory : IQueryHandler<GetProductHistory, IListPaged<ProductHistory>>
{
    private readonly IMongoCollection<ProductHistory> collection;

    public HandleGetProductHistory(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<ProductHistory>();
        collection = database.GetCollection<ProductHistory>(collectionName);
    }

    public async Task<IListPaged<ProductHistory>> Handle(GetProductHistory request, CancellationToken cancellationToken)
    {
        var (productId, option) = request;

        var filter = Builders<ProductHistory>.Filter.Eq(e => e.AggregateId, productId);

        return await collection.FindWithPagingAsync(filter, option, cancellationToken);
    }
}