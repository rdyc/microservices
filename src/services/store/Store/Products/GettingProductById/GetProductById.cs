using FW.Core.Exceptions;
using FW.Core.MongoDB;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Store.Products.GettingProductById;

public record GetProductById(Guid Id) : IQuery<ProductDetail>;

internal class HandleGetProductById : IQueryHandler<GetProductById, ProductDetail>
{
    private readonly IMongoCollection<ProductDetail> collection;

    public HandleGetProductById(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<ProductDetail>();
        collection = database.GetCollection<ProductDetail>(collectionName);
    }

    public async Task<ProductDetail> Handle(GetProductById request, CancellationToken cancellationToken)
    {
        var filter = Builders<ProductDetail>.Filter.Eq(e => e.Id, request.Id);
        var result = await collection.Find(filter).SingleOrDefaultAsync(cancellationToken);

        if (result == null)
            throw AggregateNotFoundException.For<ProductDetail>(request.Id);

        return result;
    }
}