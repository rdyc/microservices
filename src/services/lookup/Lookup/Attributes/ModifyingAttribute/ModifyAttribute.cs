using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Attributes.ModifyingAttribute;

public record ModifyAttribute(Guid? Id, string Name, AttributeType Type, string Unit) : AttributeCommand(Id, Name, Type, Unit, default);

internal class ValidateModifyAttribute : AttributeValidator<ModifyAttribute>
{
    public ValidateModifyAttribute(IMongoDatabase database) : base(database)
    {
        ValidateId();
        ValidateName();
        ValidateUnit(true);
    }
}

internal class HandleModifyAttribute : ICommandHandler<ModifyAttribute>
{
    private readonly IEventStoreDBRepository<Attribute> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleModifyAttribute(IEventStoreDBRepository<Attribute> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(ModifyAttribute request, CancellationToken cancellationToken)
    {
        var (id, name, type, unit, _) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                id.Value,
                (attribute) => attribute.Modify(name, type, unit),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}