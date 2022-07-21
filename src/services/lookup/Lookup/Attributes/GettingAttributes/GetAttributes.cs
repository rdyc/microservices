using FW.Core.MongoDB;
using FW.Core.MongoDB.Extensions;
using FW.Core.Pagination;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Lookup.Attributes.GettingAttributes;

public record GetAttributes(
    int Index,
    int Size
) : IQuery<IListPaged<AttributeShortInfo>>
{
    public static GetAttributes Create(int index, int size) =>
        new(index, size);
}

internal class HandleGetAttributes : IQueryHandler<GetAttributes, IListPaged<AttributeShortInfo>>
{
    private readonly IMongoCollection<AttributeShortInfo> collection;

    public HandleGetAttributes(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<AttributeShortInfo>();
        collection = database.GetCollection<AttributeShortInfo>(collectionName);
    }

    public async Task<IListPaged<AttributeShortInfo>> Handle(GetAttributes request, CancellationToken cancellationToken)
    {
        var (index, size) = request;

        var filter = Builders<AttributeShortInfo>.Filter.Empty;

        return await collection.FindWithPagingAsync(filter, index, size, cancellationToken);
    }
}