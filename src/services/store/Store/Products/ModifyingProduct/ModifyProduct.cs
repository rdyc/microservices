using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Store.Products.GettingProducts;

namespace Store.Products.ModifyingProduct;

public record ModifyProduct(
    Guid Id,
    string Sku,
    string Name,
    string Description
) : IProduct, ICommand
{
    public static ModifyProduct Create(
        Guid id,
        string sku,
        string name,
        string description
    ) => new(id, sku, name, description);
}

internal class ValidateModifyProduct : AbstractValidator<ModifyProduct>
{
    public ValidateModifyProduct(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<ProductShortInfo>();
        var collection = database.GetCollection<ProductShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.Id).NotEmpty().MustExistProduct(collection);
        RuleFor(p => p.Sku).NotEmpty().MustUniqueProductSku(collection, true);
        RuleFor(p => p.Name).NotEmpty().MaximumLength(50);
        RuleFor(p => p.Description).NotEmpty().MaximumLength(250);
    }
}

internal class HandleModifyProduct : ICommandHandler<ModifyProduct>
{
    private readonly IEventStoreDBRepository<Product> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleModifyProduct(IEventStoreDBRepository<Product> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(ModifyProduct request, CancellationToken cancellationToken)
    {
        var (id, sku, name, description) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                id,
                (product) => product.Modify(sku, name, description),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}