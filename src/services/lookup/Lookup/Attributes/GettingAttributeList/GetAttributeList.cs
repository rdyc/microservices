using FW.Core.MongoDB;
using FW.Core.Pagination;
using FW.Core.Queries;
using Lookup.Attributes.GettingAttributes;
using MongoDB.Driver;

namespace Lookup.Attributes.GettingAttributeList;

public record GetAttributeList(
    LookupStatus? Status
) : IQuery<IListUnpaged<AttributeShortInfo>>
{
    public static GetAttributeList Create(LookupStatus? status) =>
        new(status);
}

internal class HandleGetAttributeList : IQueryHandler<GetAttributeList, IListUnpaged<AttributeShortInfo>>
{
    private readonly IMongoCollection<AttributeShortInfo> collection;

    public HandleGetAttributeList(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<AttributeShortInfo>();
        collection = database.GetCollection<AttributeShortInfo>(collectionName);
    }

    public async Task<IListUnpaged<AttributeShortInfo>> Handle(GetAttributeList request, CancellationToken cancellationToken)
    {
        var filter = Builders<AttributeShortInfo>.Filter.Eq(e => e.Status, request.Status.HasValue ? request.Status : LookupStatus.Active);

        var data = await collection.Find(filter).ToListAsync(cancellationToken);

        return ListUnpaged<AttributeShortInfo>.Create(data);
    }
}