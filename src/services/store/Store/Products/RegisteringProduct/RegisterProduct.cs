using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Store.Products.GettingProducts;

namespace Store.Products.RegisteringProduct;

public record RegisterProduct(
    Guid ProductId,
    string SKU,
    string Name,
    string Description
) : IProduct, ICommand;

internal class ValidateRegisterProduct : AbstractValidator<RegisterProduct>
{
    private readonly IMongoCollection<ProductShortInfo> collection;

    public ValidateRegisterProduct(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<ProductShortInfo>();
        collection = database.GetCollection<ProductShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.SKU).NotEmpty().MustUniqueProductSKU(collection);
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.Description).NotEmpty();
    }
}

internal class HandleRegisterProduct : ICommandHandler<RegisterProduct>
{
    private readonly IEventStoreDBRepository<Product> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRegisterProduct(IEventStoreDBRepository<Product> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RegisterProduct request, CancellationToken cancellationToken)
    {
        var (productId, sku, name, description) = request;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                Product.Register(productId, sku, name, description),
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}