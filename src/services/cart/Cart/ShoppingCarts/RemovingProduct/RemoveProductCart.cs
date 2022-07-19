using Cart.ShoppingCarts.GettingCartById;
using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;

namespace Cart.ShoppingCarts.RemovingProduct;

public record RemoveProductCart(
    Guid CartId,
    Guid ProductId
) : ICommand
{
    public static RemoveProductCart Create(Guid? cartId, Guid? productId)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        if (productId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));

        return new RemoveProductCart(cartId.Value, productId.Value);
    }
}

internal class ValidateRemoveProductCart : AbstractValidator<RemoveProductCart>
{
    public ValidateRemoveProductCart()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.CartId).NotEmpty();
        RuleFor(p => p.ProductId).NotEmpty();
    }
}

internal class HandleRemoveProductCart : ICommandHandler<RemoveProductCart>
{
    private readonly IMongoCollection<ShoppingCartDetails> collection;
    private readonly IEventStoreDBRepository<ShoppingCart> cartRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRemoveProductCart(
        IMongoDatabase database,
        IEventStoreDBRepository<ShoppingCart> cartRepository,
        IEventStoreDBAppendScope scope
    )
    {
        var collectionName = MongoHelper.GetCollectionName<ShoppingCartDetails>();
        this.collection = database.GetCollection<ShoppingCartDetails>(collectionName);
        this.cartRepository = cartRepository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RemoveProductCart request, CancellationToken cancellationToken)
    {
        var (cartId, productId) = request;

        var cart = await collection.Find(e => e.Id.Equals(cartId))
            .SingleAsync(cancellationToken);

        var product = cart.Products.Single(e => e.ProductId.Equals(productId));

        await scope.Do((expectedRevision, eventMetadata) =>
            cartRepository.GetAndUpdate(
                cartId,
                cart => cart.RemoveProduct(ShoppingCartProduct.From(
                    product.ProductId,
                    product.Sku,
                    product.Name,
                    product.Price,
                    product.Quantity
                )),
                expectedRevision,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}