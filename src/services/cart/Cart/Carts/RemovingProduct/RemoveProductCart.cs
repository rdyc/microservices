using Cart.Carts.GettingCartById;
using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;

namespace Cart.Carts.RemovingProduct;

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
    private readonly IMongoCollection<CartDetails> collection;
    private readonly IEventStoreDBRepository<Cart> cartRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRemoveProductCart(
        IMongoDatabase database,
        IEventStoreDBRepository<Cart> cartRepository,
        IEventStoreDBAppendScope scope
    )
    {
        var collectionName = MongoHelper.GetCollectionName<CartDetails>();
        this.collection = database.GetCollection<CartDetails>(collectionName);
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
                cart => cart.RemoveProduct(CartProduct.From(
                    product.ProductId,
                    product.Sku,
                    product.Name,
                    product.Quantity,
                    CartCurrency.Create(
                        product.Currency.Id,
                        product.Currency.Name,
                        product.Currency.Code,
                        product.Currency.Symbol),
                    product.Price
                )),
                expectedRevision,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}