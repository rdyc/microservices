using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Currencies.RegisteringCurrency;

public record RegisterCurrency(Guid? Id, string Name, string Code, string Symbol, LookupStatus Status) : CurrencyCommand(Id, Name, Code, Symbol, Status);

internal class ValidateRegisterCurrency : CurrencyValidator<RegisterCurrency>
{
    public ValidateRegisterCurrency(IMongoDatabase database) : base(database)
    {
        ValidateName();
        ValidateCode();
        ValidateSymbol();
    }
}

internal class HandleRegisterCurrency : ICommandHandler<RegisterCurrency>
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
        var (id, name, code, symbol, status) = command;

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