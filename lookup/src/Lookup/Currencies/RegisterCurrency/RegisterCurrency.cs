using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Lookup.Currencies.RegisterCurrency;

public record RegisterCurrency(Guid Id, string Name, string Code, string Symbol) : IRequest;

internal class HandleRegisterCurrency : IRequestHandler<RegisterCurrency>
{
    private readonly IEventStoreDBRepository<Currency> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRegisterCurrency(IEventStoreDBRepository<Currency> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RegisterCurrency command, CancellationToken cancellationToken)
    {
        var (Id, Code, Name, Symbol) = command;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                Currency.Register(Id, Code, Name, Symbol),
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}