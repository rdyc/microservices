using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;

namespace Lookup.Currencies.Registering;

public record RegisterCurrency(string Name, string Code, string Symbol) : CurrencyCommand(Guid.NewGuid(), Name, Code, Symbol);

internal class ValidateRegister : CurrencyValidator<RegisterCurrency>
{
    public ValidateRegister()
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
        var (Id, Code, Name, Symbol) = command;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                Currency.Register(Id, Code, Name, Symbol),
                eventMetadata,
                cancellationToken
            )
        );

        return Id.Value;
    }
}