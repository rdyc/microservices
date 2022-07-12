using FW.Core.Exceptions;
using FW.Core.MongoDB;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Store.Products.GettingProductHistory;

public record GetProductHistory(Guid Id) : IQuery<ProductHistory>;

internal class HandleGetProductHistory : IQueryHandler<GetProductHistory, ProductHistory>
{
    private readonly IMongoCollection<ProductHistory> collection;

    public HandleGetProductHistory(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<ProductHistory>();
        collection = database.GetCollection<ProductHistory>(collectionName);
    }

    public async Task<ProductHistory> Handle(GetProductHistory request, CancellationToken cancellationToken)
    {
        var filter = Builders<ProductHistory>.Filter.Eq(e => e.Id, request.Id);
        var result = await collection.Find(filter).SingleOrDefaultAsync(cancellationToken);

        if (result == null)
            throw AggregateNotFoundException.For<ProductHistory>(request.Id);

        return result;
    }
}