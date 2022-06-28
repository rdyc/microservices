using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Lookup.Currencies.Removing;

public record RemoveCurrency(Guid? Id) : CurrencyCommand(Id, default, default, default, default);

internal class ValidateRemove : CurrencyValidator<RemoveCurrency>
{
    public ValidateRemove()
    {
        ValidateId();
    }
}

internal class HandleRemoveCurrency : IRequestHandler<RemoveCurrency, Guid>
{
    private readonly IEventStoreDBRepository<Currency> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRemoveCurrency(IEventStoreDBRepository<Currency> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Guid> Handle(RemoveCurrency command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                command.Id.Value,
                (currency) => currency.Remove(),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return command.Id.Value;
    }
}