using System.Threading;
using System.Threading.Tasks;
using Core.Commands;
using Core.EventStoreDB.OptimisticConcurrency;
using Core.EventStoreDB.Repository;
using MediatR;
using Product.Domain.Models;

namespace Product.Domain.Commands;

public record CreateCurrencyCmd(string Name, string Code, string Symbol) : ICommand;

internal class CreateCurrencyCmdHandler : ICommandHandler<CreateCurrencyCmd>
{
    private readonly IEventStoreDBRepository<CurrencyModel> repository;
    private readonly IEventStoreDBAppendScope scope;

    public CreateCurrencyCmdHandler(
        IEventStoreDBRepository<CurrencyModel> repository,
        IEventStoreDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(CreateCurrencyCmd request, CancellationToken cancellationToken)
    {
        var (Name, Code, Symbol) = request;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                CurrencyModel.Create(Name, Code, Symbol),
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}