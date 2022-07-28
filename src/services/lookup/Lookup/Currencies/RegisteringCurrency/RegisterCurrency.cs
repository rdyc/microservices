using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using Lookup.Currencies.GettingCurrencies;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Currencies.RegisteringCurrency;

public record RegisterCurrency(
    Guid Id,
    string Name,
    string Code,
    string Symbol,
    LookupStatus Status
) : ICurrency, ICommand
{
    public static RegisterCurrency Create(
        Guid id,
        string name,
        string code,
        string symbol,
        LookupStatus status
    ) => new(id, name, code, symbol, status);
}

internal class ValidateRegisterCurrency : AbstractValidator<RegisterCurrency>
{
    private readonly IMongoCollection<CurrencyShortInfo> collection;

    public ValidateRegisterCurrency(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<CurrencyShortInfo>();
        collection = database.GetCollection<CurrencyShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.Code).NotEmpty().MaximumLength(3)
            .MustUniqueCurrencyCode(collection);
        RuleFor(p => p.Symbol).NotEmpty().MaximumLength(3);
    }
}

internal class HandleRegisterCurrency : ICommandHandler<RegisterCurrency>
{
    private readonly IEventStoreDBRepository<Currency> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRegisterCurrency(
        IEventStoreDBRepository<Currency> repository,
        IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RegisterCurrency request, CancellationToken cancellationToken)
    {
        var (id, name, code, symbol, status) = request;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                Currency.Register(id, name, code, symbol, status),
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}