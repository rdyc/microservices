using FW.Core.Exceptions;
using FW.Core.MongoDB;
using FW.Core.Queries;
using Lookup.Attributes.GettingAttributes;
using MongoDB.Driver;

namespace Lookup.Attributes.GettingAttributeById;

public record GetAttributeById(
    Guid Id
) : IQuery<AttributeShortInfo>
{
    public static GetAttributeById Create(Guid id) =>
        new(id);
}

internal class HandleGetAttributeById : IQueryHandler<GetAttributeById, AttributeShortInfo>
{
    private readonly IMongoCollection<AttributeShortInfo> collection;

    public HandleGetAttributeById(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<AttributeShortInfo>();
        collection = database.GetCollection<AttributeShortInfo>(collectionName);
    }

    public async Task<AttributeShortInfo> Handle(GetAttributeById request, CancellationToken cancellationToken)
    {
        var filter = Builders<AttributeShortInfo>.Filter.Eq(e => e.Id, request.Id);

        var result = await collection.Find(filter).SingleOrDefaultAsync(cancellationToken);

        if (result == null)
            throw AggregateNotFoundException.For<AttributeShortInfo>(request.Id);

        return result;
    }
}