using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using Lookup.Attributes.GettingAttributes;
using MediatR;
using MongoDB.Driver;

namespace Lookup.Attributes.RegisteringAttribute;

public record RegisterAttribute(
    Guid AttributeId,
    string Name,
    AttributeType Type,
    string Unit,
    LookupStatus Status
) : IAttribute, ICommand;

internal class ValidateRegisterAttribute : AbstractValidator<RegisterAttribute>
{
    private readonly IMongoCollection<AttributeShortInfo> collection;

    public ValidateRegisterAttribute(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<AttributeShortInfo>();
        collection = database.GetCollection<AttributeShortInfo>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.AttributeId).NotEmpty();
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.Unit).NotEmpty().MaximumLength(5)
            .MustUniqueAttributeUnit(collection);
    }
}

internal class HandleRegisterAttribute : ICommandHandler<RegisterAttribute>
{
    private readonly IEventStoreDBRepository<Attribute> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRegisterAttribute(
        IEventStoreDBRepository<Attribute> repository,
        IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RegisterAttribute request, CancellationToken cancellationToken)
    {
        var (attributeId, name, type, unit, status) = request;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                Attribute.Register(attributeId, name, type, unit, status),
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}