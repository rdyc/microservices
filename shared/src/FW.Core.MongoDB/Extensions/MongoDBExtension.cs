using FW.Core.Pagination;
using MongoDB.Driver;

namespace FW.Core.MongoDB.Extensions;

public static class MongoDBExtension
{
    public static async Task<IListPaged<TDocument>> FindWithPagingAsync<TDocument>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument> filter,
        int index,
        int size,
        CancellationToken cancellationToken = default
    ) where TDocument : class
    {
        var count = collection.CountDocumentsAsync(filter, null, cancellationToken);
        var records = collection.Find(filter)
            .Skip(index * size)
            .Limit(size)
            .ToListAsync(cancellationToken);

        await Task.WhenAll(count, records);

        return ListPaged<TDocument>.Create(records.Result, Metadata.Create(count.Result, index, size));
    }
}