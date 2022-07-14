using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Store.Products.GettingProductHistory;

public record GetProductHistory(
    Guid Id,
    int PageNumber,
    int PageSize
) : IQuery<IListPaged<ProductHistory>>
{
    public static GetProductHistory Create(Guid id, int? pageNumber = 0, int? pageSize = 10)
    {
        if (pageNumber is null or < 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize is null or < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        return new GetProductHistory(id, pageNumber.Value, pageSize.Value);
    }
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
        var (id, index, size) = request;

        var filter = Builders<ProductHistory>.Filter.Eq(e => e.AggregateId, request.Id);

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}