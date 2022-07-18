using Cart.ShoppingCarts.GettingCartById;
using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;
using MongoDB.Driver;

namespace Cart.ShoppingCarts.RemovingProduct;

public record RemoveProduct(
    Guid CartId,
    Guid ProductId
) : ICommand
{
    public static RemoveProduct Create(Guid? cartId, Guid? productId)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        if (productId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));

        return new RemoveProduct(cartId.Value, productId.Value);
    }
}

internal class ValidateRemoveProduct : AbstractValidator<RemoveProduct>
{
    public ValidateRemoveProduct()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.CartId).NotEmpty();
        RuleFor(p => p.ProductId).NotEmpty();
    }
}

internal class HandleRemoveProduct : ICommandHandler<RemoveProduct>
{
    private readonly IMongoCollection<ShoppingCartDetails> collection;
    private readonly IEventStoreDBRepository<ShoppingCart> cartRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRemoveProduct(
        IMongoCollection<ShoppingCartDetails> collection,
        IEventStoreDBRepository<ShoppingCart> cartRepository,
        IEventStoreDBAppendScope scope
    )
    {
        this.collection = collection;
        this.cartRepository = cartRepository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RemoveProduct request, CancellationToken cancellationToken)
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