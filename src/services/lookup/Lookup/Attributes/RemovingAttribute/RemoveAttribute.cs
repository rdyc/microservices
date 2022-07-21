using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using Lookup.Attributes.GettingAttributes;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Attributes.RemovingAttribute;

public record RemoveAttribute(
    Guid AttributeId
) : IAttribute, ICommand
{
    public static RemoveAttribute Create(Guid attributeId)
        => new(attributeId);
}

internal class ValidateRemoveAttribute : AbstractValidator<RemoveAttribute>
{
    private readonly IMongoCollection<AttributeShortInfo> collection;

    public ValidateRemoveAttribute(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<AttributeShortInfo>();
        collection = database.GetCollection<AttributeShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.AttributeId).NotEmpty()
            .MustExistAttribute(collection);
    }
}

internal class HandleRemoveAttribute : ICommandHandler<RemoveAttribute>
{
    private readonly IEventStoreDBRepository<Attribute> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRemoveAttribute(
        IEventStoreDBRepository<Attribute> repository,
        IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RemoveAttribute request, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                request.AttributeId,
                (attribute) => attribute.Remove(),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}