using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Store.Lookup.Attributes;
using Attribute = Store.Lookup.Attributes.Attribute;

namespace Store.Products.RemovingAttribute;

public record RemoveProductAttribute(
    Guid ProductId,
    Guid AttributeId
) : ICommand
{
    public static RemoveProductAttribute Create(Guid productId, Guid attributeId)
    {
        if (productId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(productId));

        if (attributeId == Guid.Empty)
            throw new ArgumentNullException(nameof(attributeId));

        return new(productId, attributeId);
    }
}

internal class ValidateRemoveProductAttribute : AbstractValidator<RemoveProductAttribute>
{
    public ValidateRemoveProductAttribute(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<Attribute>();
        var collection = database.GetCollection<Attribute>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.AttributeId).NotEmpty().MustExistAttribute(collection);
    }
}

internal class HandleRemoveAttribute : ICommandHandler<RemoveProductAttribute>
{
    private readonly IEventStoreDBRepository<Product> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleRemoveAttribute(IEventStoreDBRepository<Product> repository, IEventStoreDBAppendScope scope)
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(RemoveProductAttribute request, CancellationToken cancellationToken)
    {
        var (productId, attributeId) = request;

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                productId,
                (product) => product.RemoveAttribute(attributeId),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}