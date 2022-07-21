using Cart.Products;
using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;

namespace Cart.ShoppingCarts.AddingProduct;

public record AddProductCart(
    Guid CartId,
    Guid ProductId,
    int Quantity
) : IProduct, ICommand
{
    public static AddProductCart Create(Guid? cartId, Guid? productId, int quantity)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        if (productId == null || productId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));

        if (quantity <= 0)
            throw new IndexOutOfRangeException(nameof(quantity));

        return new(cartId.Value, productId.Value, quantity);
    }
}

internal class ValidateAddProductCart : AbstractValidator<AddProductCart>
{
    public ValidateAddProductCart(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<Product>();
        var collection = database.GetCollection<Product>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.CartId).NotEmpty();

        RuleFor(p => p.ProductId).NotEmpty()
            .MustExistProduct(collection);

        RuleFor(p => p.Quantity).GreaterThan(0)
            .MustInStockProduct(collection);
    }
}

internal class HandleAddProductCart : ICommandHandler<AddProductCart>
{
    private readonly IMongoCollection<Product> collection;
    private readonly IEventStoreDBRepository<ShoppingCart> cartRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleAddProductCart(
        IMongoDatabase database,
        IEventStoreDBRepository<ShoppingCart> cartRepository,
        IEventStoreDBAppendScope scope
    )
    {
        var collectionName = MongoHelper.GetCollectionName<Product>();
        this.collection = database.GetCollection<Product>(collectionName);
        this.cartRepository = cartRepository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(AddProductCart request, CancellationToken cancellationToken)
    {
        var (cartId, productId, quantity) = request;

        var product = await collection.Find(e => e.Id.Equals(productId))
            .SingleAsync(cancellationToken);

        await scope.Do((expectedRevision, eventMetadata) =>
            cartRepository.GetAndUpdate(
                cartId,
                cart => cart.AddProduct(ShoppingCartProduct.From(
                    product.Id,
                    product.Sku,
                    product.Name,
                    quantity,
                    ShoppingCartCurrency.Create(
                        product.Currency.Id,
                        product.Currency.Name,
                        product.Currency.Code,
                        product.Currency.Symbol),
                    product.Price)),
                expectedRevision,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}