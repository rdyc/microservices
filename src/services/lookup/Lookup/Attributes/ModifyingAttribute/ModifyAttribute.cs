using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using Lookup.Attributes.GettingAttributes;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Attributes.ModifyingAttribute;

public record ModifyAttribute(
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit
) : IAttribute, ICommand
{
    public static ModifyAttribute Create(
        Guid attributeId,
        string name,
        AttributeType type,
        string unit
    ) => new(attributeId, name, type, unit);
}

internal class ValidateModifyAttribute : AbstractValidator<ModifyAttribute>
{
    private readonly IMongoCollection<AttributeShortInfo> collection;
    public ValidateModifyAttribute(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<AttributeShortInfo>();
        collection = database.GetCollection<AttributeShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.AttributeId).NotEmpty()
            .MustExistAttribute(collection);
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.Unit).NotEmpty().MaximumLength(5)
            .MustUniqueAttributeUnit(collection, true);
    }
}

internal class HandleModifyAttribute : ICommandHandler<ModifyAttribute>
{
    private readonly IEventStoreDBRepository<Attribute> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleModifyAttribute(
        IEventStoreDBRepository<Attribute> repository,
        IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(ModifyAttribute request, CancellationToken cancellationToken)
    {
        var (attributeId, name, type, unit) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                attributeId,
                (attribute) => attribute.Modify(name, type, unit),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}