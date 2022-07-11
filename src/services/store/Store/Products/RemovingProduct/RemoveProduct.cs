using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Store.Products.GettingProducts;

namespace Store.Products.RemovingProduct;

public record RemoveProduct(Guid Id) : ICommand;

internal class ValidateRemoveProduct : AbstractValidator<RemoveProduct>
{
    public ValidateRemoveProduct(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<ProductShortInfo>();
        var collection = database.GetCollection<ProductShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.Id).NotEmpty().MustExistProduct(collection);
    }
}

internal class HandleRemoveProduct : ICommandHandler<RemoveProduct>
{
    private readonly IEventStoreDBRepository<Product> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRemoveProduct(IEventStoreDBRepository<Product> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RemoveProduct request, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                request.Id,
                (product) => product.Remove(),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}