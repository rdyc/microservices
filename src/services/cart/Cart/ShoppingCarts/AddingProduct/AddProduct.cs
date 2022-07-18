using Cart.Products;
using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;

namespace Cart.ShoppingCarts.AddingProduct;

public record AddProduct(
    Guid CartId,
    Guid ProductId,
    int Quantity
) : IProduct, ICommand
{
    public static AddProduct Create(Guid? cartId, Guid? productId, int quantity)
    {
        if (cartId == null || cartId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(cartId));

        if (productId == null || productId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));

        if (quantity <= 0)
            throw new IndexOutOfRangeException(nameof(quantity));

        return new AddProduct(cartId.Value, productId.Value, quantity);
    }
}

internal class ValidateAddProduct : AbstractValidator<AddProduct>
{
    public ValidateAddProduct(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<Product>();
        var collection = database.GetCollection<Product>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.CartId).NotEmpty();

        RuleFor(p => p.ProductId).NotEmpty()
            .MustExistProduct(collection);

        RuleFor(p => p.Quantity).GreaterThan(0);
    }
}

internal class AddProductValidator : AbstractValidator<AddProduct>
{
    public AddProductValidator(IMongoDatabase database)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        var collectionName = MongoHelper.GetCollectionName<Product>();
        var collection = database.GetCollection<Product>(collectionName);

        RuleFor(p => p.CartId).NotEmpty();

        RuleFor(p => p.ProductId).NotEmpty()
            .MustExistProduct(collection);

        RuleFor(p => p.Quantity).GreaterThan(0)
            .MustInStockProduct(collection);
    }
}

internal class HandleAddProduct : ICommandHandler<AddProduct>
{
    private readonly IMongoCollection<Product> collection;
    private readonly IEventStoreDBRepository<ShoppingCart> cartRepository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleAddProduct(
        IMongoCollection<Product> collection,
        IEventStoreDBRepository<ShoppingCart> cartRepository,
        IEventStoreDBAppendScope scope
    )
    {
        this.collection = collection;
        this.cartRepository = cartRepository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(AddProduct request, CancellationToken cancellationToken)
    {
        var (cartId, productId, quantity) = request;
        var product = await collection.Find(e => e.Id.Equals(productId))
            .SingleOrDefaultAsync(cancellationToken);

        await scope.Do((expectedRevision, eventMetadata) =>
            cartRepository.GetAndUpdate(
                cartId,
                cart => cart.AddProduct(ShoppingCartProduct.From(
                    product.Id,
                    product.Sku,
                    product.Name,
                    product.Price,
                    quantity)),
                expectedRevision,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}