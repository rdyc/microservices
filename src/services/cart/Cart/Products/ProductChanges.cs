using FW.Core.Commands;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;

namespace Cart.Products;

public record CreateProduct(Guid Id, string Sku, string Name, string Description, ProductStatus Status) : ICommand
{
    public static CreateProduct Create(Guid id, string sku, string name, string description, ProductStatus status) =>
        new(id, sku, name, description, status);
}

public record UpdateProduct(Guid Id, string Sku, string Name, string Description) : ICommand
{
    public static UpdateProduct Create(Guid id, string sku, string name, string description) =>
        new(id, sku, name, description);
}

public record DeleteProduct(Guid Id) : ICommand
{
    public static DeleteProduct Create(Guid id) => new(id);
}

internal class HandleProductChanges :
    ICommandHandler<CreateProduct>,
    ICommandHandler<UpdateProduct>,
    ICommandHandler<DeleteProduct>
{
    private readonly IMongoCollection<Product> collection;

    public HandleProductChanges(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<Product>();
        collection = database.GetCollection<Product>(collectionName);
    }

    public async Task<Unit> Handle(CreateProduct request, CancellationToken cancellationToken)
    {
        var (id, sku, name, description, status) = request;

        var data = new Product
        {
            Id = id,
            Sku = sku,
            Name = name,
            Description = description,
            Status = status
        };

        await collection.InsertOneAsync(data, default, cancellationToken);

        return Unit.Value;
    }

    public async Task<Unit> Handle(UpdateProduct request, CancellationToken cancellationToken)
    {
        var (id, sku, name, description) = request;

        var update = Builders<Product>.Update
            .Set(e => e.Sku, sku)
            .Set(e => e.Name, name)
            .Set(e => e.Description, description);

        await collection.UpdateOneAsync(e => e.Id == id, update, new UpdateOptions { IsUpsert = true }, cancellationToken);

        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteProduct request, CancellationToken cancellationToken)
    {
        await collection.DeleteOneAsync(e => e.Id == request.Id, cancellationToken);

        return Unit.Value;
    }
}