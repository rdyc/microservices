using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Currencies.Removing;

public record RemoveCurrency(Guid? Id) : CurrencyCommand(Id, default, default, default, default);

internal class ValidateRemoveCurrency : CurrencyValidator<RemoveCurrency>
{
    public ValidateRemoveCurrency(IMongoDatabase database) : base(database)
    {
        ValidateId();
    }
}

internal class HandleRemoveCurrency : ICommandHandler<RemoveCurrency>
{
    private readonly IEventStoreDBRepository<Currency> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRemoveCurrency(IEventStoreDBRepository<Currency> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RemoveCurrency command, CancellationToken cancellationToken)
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

        return Unit.Value;
    }
}