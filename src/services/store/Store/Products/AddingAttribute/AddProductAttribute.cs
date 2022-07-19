using FluentValidation;
using FW.Core.Commands;
using FW.Core.EventStoreDB.OptimisticConcurrency;
using FW.Core.EventStoreDB.Repository;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;
using Store.Lookup.Attributes;
using Attribute = Store.Lookup.Attributes.Attribute;

namespace Store.Products.AddingAttribute;

public record AddProductAttribute(
    Guid ProductId,
    Guid AttributeId,
    string Value
) : ICommand
{
    public static AddProductAttribute Create(Guid productId, Guid attributeId, string value)
    {
        if (productId == Guid.Empty)
            throw new ArgumentNullException(nameof(productId));

        if (attributeId == Guid.Empty)
            throw new ArgumentNullException(nameof(attributeId));

        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException(nameof(value));

        return new(productId, attributeId, value);
    }
}

internal class ValidateAddAttribute : AbstractValidator<AddProductAttribute>
{
    public ValidateAddAttribute(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<Attribute>();
        var collection = database.GetCollection<Attribute>(collectionName);

        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(p => p.AttributeId).NotEmpty().MustExistAttribute(collection);
    }
}

internal class HandleAddProductAttribute : ICommandHandler<AddProductAttribute>
{
    private readonly IMongoCollection<Attribute> collection;
    private readonly IEventStoreDBRepository<Product> repository;
    private readonly IEventStoreDBAppendScope scope;

    public HandleAddProductAttribute(
        IMongoDatabase database,
        IEventStoreDBRepository<Product> repository,
        IEventStoreDBAppendScope scope)
    {
        var collectionName = MongoHelper.GetCollectionName<Attribute>();
        this.collection = database.GetCollection<Attribute>(collectionName);
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(AddProductAttribute request, CancellationToken cancellationToken)
    {
        var (productId, attributeId, value) = request;

        var attribute = await collection
            .Find(e => e.Id.Equals(attributeId))
            .SingleOrDefaultAsync(cancellationToken);

        var productAttribute = new ProductAttribute(
            attribute.Id,
            attribute.Name,
            attribute.Type,
            attribute.Unit,
            value);

        await scope.Do((expectedVersion, eventMetadata) =>
            repository.GetAndUpdate(
                productId,
                (product) => product.AddAttribute(productAttribute),
                expectedVersion,
                eventMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}