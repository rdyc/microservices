using FW.Core.Exceptions;
using FW.Core.MongoDB;
using FW.Core.Queries;
using MongoDB.Driver;

namespace Cart.ShoppingCarts.GettingCartById;

public record GetCartById(
    Guid CartId
) : IQuery<ShoppingCartDetails>
{
    public static GetCartById Create(Guid? cartId)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        return new(cartId.Value);
    }
}

internal class HandleGetCartById : IQueryHandler<GetCartById, ShoppingCartDetails>
{
    private readonly IMongoCollection<ShoppingCartDetails> collection;

    public HandleGetCartById(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<ShoppingCartDetails>();
        this.collection = database.GetCollection<ShoppingCartDetails>(collectionName);
    }

    public async Task<ShoppingCartDetails> Handle(GetCartById request, CancellationToken cancellationToken)
    {
        var filter = Builders<ShoppingCartDetails>.Filter.Eq(e => e.Id, request.CartId);
        var cart = await collection.Find(filter).SingleOrDefaultAsync(cancellationToken);

        if (cart == null)
            throw AggregateNotFoundException.For<ShoppingCartDetails>(request.CartId);

        return cart;
    }
}
