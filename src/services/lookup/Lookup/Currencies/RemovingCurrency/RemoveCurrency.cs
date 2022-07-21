using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using Lookup.Currencies.GettingCurrencies;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Currencies.RemovingCurrency;

public record RemoveCurrency(
    Guid Id
) : ICurrency, ICommand
{
    public static RemoveCurrency Create(Guid id) => new(id);
}

internal class ValidateRemoveCurrency : AbstractValidator<RemoveCurrency>
{
    private readonly IMongoCollection<CurrencyShortInfo> collection;

    public ValidateRemoveCurrency(IMongoDatabase database)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        var collectionName = MongoHelper.GetCollectionName<CurrencyShortInfo>();
        collection = database.GetCollection<CurrencyShortInfo>(collectionName);

        RuleFor(p => p.Id).NotEmpty()
            .MustExistCurrency(collection);
    }
}

internal class HandleRemoveCurrency : ICommandHandler<RemoveCurrency>
{
    private readonly IEventStoreDBRepository<Currency> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRemoveCurrency(
        IEventStoreDBRepository<Currency> repository,
        IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RemoveCurrency request, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                request.Id,
                (currency) => currency.Remove(),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}