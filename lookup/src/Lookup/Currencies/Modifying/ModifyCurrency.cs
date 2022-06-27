using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Lookup.Currencies.Modifying;

public record ModifyCurrency(Guid? Id, string Name, string Code, string Symbol) : CurrencyCommand(Id, Name, Code, Symbol);

internal class ValidateModify : CurrencyValidator<ModifyCurrency>
{
    public ValidateModify()
    {
        ValidateId();
        ValidateName();
        ValidateCode();
        ValidateSymbol();
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
        var (Id, Code, Name, Symbol) = command;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                Id.Value,
                (currency) => currency.Modify(Name, Code, Symbol),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Id.Value;
    }
}