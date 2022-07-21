using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using Lookup.Currencies.GettingCurrencies;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Currencies.ModifyingCurrency;

public record ModifyCurrency(
    Guid Id,
    string Name,
    string Code,
    string Symbol
) : ICurrency, ICommand
{
    public static ModifyCurrency Create(
        Guid id,
        string name,
        string code,
        string symbol
    ) => new(id, name, code, symbol);
}

internal class ValidateModifyCurrency : AbstractValidator<ModifyCurrency>
{
    private readonly IMongoCollection<CurrencyShortInfo> collection;

    public ValidateModifyCurrency(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<CurrencyShortInfo>();
        collection = database.GetCollection<CurrencyShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.Id).NotEmpty()
            .MustExistCurrency(collection);
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.Code).NotEmpty().MaximumLength(3)
            .MustUniqueCurrencyCode(collection, true);
        RuleFor(p => p.Symbol).NotEmpty().MaximumLength(3);
    }
}

internal class HandleModifyCurrency : ICommandHandler<ModifyCurrency>
{
    private readonly IEventStoreDBRepository<Currency> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleModifyCurrency(
        IEventStoreDBRepository<Currency> repository,
        IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(ModifyCurrency request, CancellationToken cancellationToken)
    {
        var (currencyId, name, code, symbol) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                currencyId,
                (currency) => currency.Modify(name, code, symbol),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}