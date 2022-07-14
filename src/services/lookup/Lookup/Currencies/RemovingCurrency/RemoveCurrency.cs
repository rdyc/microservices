using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using Lookup.Currencies.GettingCurrencies;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Currencies.RemovingCurrency;

public record RemoveCurrency(Guid CurrencyId) : ICurrency, ICommand;

internal class ValidateRemoveCurrency : AbstractValidator<RemoveCurrency>
{
    private readonly IMongoCollection<CurrencyShortInfo> collection;

    public ValidateRemoveCurrency(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<CurrencyShortInfo>();
        collection = database.GetCollection<CurrencyShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.CurrencyId).NotEmpty()
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
                request.CurrencyId,
                (currency) => currency.Remove(),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}