using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Attributes.RegisteringAttribute;

public record RegisterAttribute(Guid? Id, string Name, AttributeType Type, string Unit, LookupStatus Status) : AttributeCommand(Id, Name, Type, Unit, Status);

internal class ValidateRegisterAttribute : AttributeValidator<RegisterAttribute>
{
    public ValidateRegisterAttribute(IMongoDatabase database) : base(database)
    {
        ValidateName();
        ValidateUnit();
    }
}

internal class HandleRegisterAttribute : ICommandHandler<RegisterAttribute>
{
    private readonly IEventStoreDBRepository<Attribute> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRegisterAttribute(IEventStoreDBRepository<Attribute> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RegisterAttribute command, CancellationToken cancellationToken)
    {
        var (id, name, type, unit, status) = command;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                Attribute.Register(id, name, type, unit, status),
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}