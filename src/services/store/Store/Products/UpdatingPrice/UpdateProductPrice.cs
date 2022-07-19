using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Store.Lookup.Currencies;

namespace Store.Products.UpdatingPrice;

public record UpdateProductPrice(
    Guid ProductId,
    Guid CurrencyId,
    decimal Price
) : ICommand;

internal class ValidateUpdateProductPrice : AbstractValidator<UpdateProductPrice>
{
    public ValidateUpdateProductPrice(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<Currency>();
        var collection = database.GetCollection<Currency>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.CurrencyId).MustExistCurrency(collection);
        RuleFor(p => p.Price).GreaterThan(0);
    }
}

internal class HandleUpdateProductPrice : ICommandHandler<UpdateProductPrice>
{
    private readonly IMongoCollection<Currency> collection;
    private readonly IEventStoreDBRepository<Product> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleUpdateProductPrice(
        IMongoDatabase database,
        IEventStoreDBRepository<Product> repository,
        IEventStoreDBAppendScope scope)
    {
        var collectionName = MongoHelper.GetCollectionName<Currency>();
        this.collection = database.GetCollection<Currency>(collectionName);
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(UpdateProductPrice request, CancellationToken cancellationToken)
    {
        var (productId, currencyId, price) = request;

        var currency = await collection
            .Find(e => e.Id.Equals(currencyId))
            .SingleOrDefaultAsync(cancellationToken);

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                productId,
                (product) => product.UpdatePrice(new CurrencyPrice(
                    currency.Id,
                    currency.Name,
                    currency.Code,
                    currency.Symbol,
                    currency.Status), price),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}