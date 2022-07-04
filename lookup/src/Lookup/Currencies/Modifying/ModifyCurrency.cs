using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Currencies.Modifying;

public record ModifyCurrency(Guid? Id, string Name, string Code, string Symbol) : CurrencyCommand(Id, Name, Code, Symbol, default);

internal class ValidateModifyCurrency : CurrencyValidator<ModifyCurrency>
{
    public ValidateModifyCurrency(IMongoDatabase database) : base(database)
    {
        ValidateId();
        ValidateName(true);
        ValidateCode(true);
        ValidateSymbol(true);
    }
}

internal class HandleModifyCurrency : IRequestHandler<ModifyCurrency, Guid>
{
    private readonly IEventStoreDBRepository<Currency> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleModifyCurrency(IEventStoreDBRepository<Currency> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Guid> Handle(ModifyCurrency command, CancellationToken cancellationToken)
    {
        var (id, name, code, symbol, _) = command;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                id.Value,
                (currency) => currency.Modify(name, code, symbol),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return id.Value;
    }
}