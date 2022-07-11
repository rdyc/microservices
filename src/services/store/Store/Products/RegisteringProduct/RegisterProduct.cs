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
    Guid Id,
    string SKU,
    string Name,
    string Description
) : ICommand;

internal class ValidateRegisterProduct : AbstractValidator<RegisterProduct>
{
    public ValidateRegisterProduct(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<ProductShortInfo>();
        var collection = database.GetCollection<ProductShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;
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
        var (id, sku, name, description) = request;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                Product.Register(id, sku, name, description),
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}