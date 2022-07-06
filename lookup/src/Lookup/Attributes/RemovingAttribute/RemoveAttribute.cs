using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Attributes.RemovingAttribute;

public record RemoveAttribute(Guid? Id) : AttributeCommand(Id, default, default, default, default);

internal class ValidateRemoveAttribute : AttributeValidator<RemoveAttribute>
{
    public ValidateRemoveAttribute(IMongoDatabase database) : base(database)
    {
        ValidateId();
    }
}

internal class HandleRemoveAttribute : ICommandHandler<RemoveAttribute>
{
    private readonly IEventStoreDBRepository<Attribute> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRemoveAttribute(IEventStoreDBRepository<Attribute> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RemoveAttribute command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                command.Id.Value,
                (attribute) => attribute.Remove(),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}