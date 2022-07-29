using FW.Core.Exceptions;
using FW.Core.MongoDB;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Cart.Carts.GettingCartById;

public record GetCartById(
    Guid CartId
) : IQuery<CartDetails>
{
    public static GetCartById Create(Guid? cartId)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        return new(cartId.Value);
    }
}

internal class HandleGetCartById : IQueryHandler<GetCartById, CartDetails>
{
    private readonly IMongoCollection<CartDetails> collection;

    public HandleGetCartById(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<CartDetails>();
        this.collection = database.GetCollection<CartDetails>(collectionName);
    }

    public async Task<CartDetails> Handle(GetCartById request, CancellationToken cancellationToken)
    {
        var filter = Builders<CartDetails>.Filter.Eq(e => e.Id, request.CartId);
        var cart = await collection.Find(filter).SingleOrDefaultAsync(cancellationToken);

        if (cart == null)
            throw AggregateNotFoundException.For<CartDetails>(request.CartId);

        return cart;
    }
}
