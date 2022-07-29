using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Lookup.Histories.GettingHistories;

public record GetHistory(
    Guid AggregateId,
    PagedOption Option
) : IQuery<IListPaged<History>>
{
    public static GetHistory Create(Guid aggregateId, int? page, int? size) =>
        new(aggregateId, PagedOption.Create(page ?? 1, size ?? 10));
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
        var (aggregateId, option) = request;

        var filter = Builders<History>.Filter.Eq(e => e.AggregateId, aggregateId);

        return await collection.FindWithPagingAsync(filter, option, cancellationToken);
    }
}