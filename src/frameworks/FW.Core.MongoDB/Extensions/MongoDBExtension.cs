using FW.Core.Pagination;
using MongoDB.Driver;

namespace FW.Core.MongoDB.Extensions;

public static class MongoDBExtension
{
    public static async Task<IListPaged<TDocument>> FindWithPagingAsync<TDocument>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument> filter,
        PagedOption option,
        CancellationToken cancellationToken
    ) where TDocument : class
    {
        var (page, size) = option;
        var count = collection.CountDocumentsAsync(filter, null, cancellationToken);
        var records = collection.Find(filter)
            .Skip((page - 1) * size)
            .Limit(size)
            .ToListAsync(cancellationToken);

        await Task.WhenAll(count, records);

        return ListPaged<TDocument>.Create(records.Result, Metadata.Create(count.Result, page, size));
    }
}