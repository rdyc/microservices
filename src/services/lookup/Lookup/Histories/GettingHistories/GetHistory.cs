using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Lookup.Histories.GettingHistories;

public record GetHistory(
    Guid AggregateId,
    int Index,
    int Size
) : IQuery<IListPaged<History>>
{
    public static GetHistory Create(
        Guid aggregateId,
        int index,
        int size
    ) => new(aggregateId, index, size);
}

internal class HandleGetHistory : IQueryHandler<GetHistory, IListPaged<History>>
{
    private readonly IMongoCollection<History> collection;

    public HandleGetHistory(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<History>();
        collection = database.GetCollection<History>(collectionName);
    }

    public async Task<IListPaged<History>> Handle(GetHistory request, CancellationToken cancellationToken)
    {
        var (aggregateId, index, size) = request;

        var filter = Builders<History>.Filter.Eq(e => e.AggregateId, aggregateId);

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}