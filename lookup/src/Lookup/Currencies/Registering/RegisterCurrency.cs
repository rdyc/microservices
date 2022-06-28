using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Lookup.Currencies.Registering;

public record RegisterCurrency(string Name, string Code, string Symbol, CurrencyStatus Status) : CurrencyCommand(Guid.NewGuid(), Name, Code, Symbol, Status);

internal class ValidateRegisterCurrency : CurrencyValidator<RegisterCurrency>
{
    public ValidateRegisterCurrency()
    {
        ValidateName();
        ValidateCode();
        ValidateSymbol();
    }
}

internal class HandleRegisterCurrency : IRequestHandler<RegisterCurrency, Guid>
{
    private readonly IEventStoreDBRepository<Currency> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRegisterCurrency(IEventStoreDBRepository<Currency> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Guid> Handle(RegisterCurrency command, CancellationToken cancellationToken)
    {
        var (id, code, name, symbol, status) = command;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                Currency.Register(id, code, name, symbol, status),
                eventMetadata,
                cancellationToken
            )
        );

        return id.Value;
    }
}