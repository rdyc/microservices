using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Commands;
using Core.EventStoreDB.OptimisticConcurrency;
using Core.EventStoreDB.Repository;
using MediatR;
using Product.Domain.Models;

namespace Product.Domain.Commands;

public record UpdateCurrencyCmd(Guid Id, string Name, string Code, string Symbol) : ICommand;

internal class UpdateCurrencyCmdHandler : ICommandHandler<UpdateCurrencyCmd>
{
    private readonly IEventStoreDBRepository<CurrencyModel> repository;
    private readonly IEventStoreDBAppendScope scope;

    public UpdateCurrencyCmdHandler(
        IEventStoreDBRepository<CurrencyModel> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(UpdateCurrencyCmd request, CancellationToken cancellationToken)
    {
        var (Id, Name, Code, Symbol) = request;

        await scope.Do((expectedRevision, eventMetadata) =>
            repository.GetAndUpdate(
                Id,
                currency => currency.Update(Id, Name, Code, Symbol),
                expectedRevision,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}