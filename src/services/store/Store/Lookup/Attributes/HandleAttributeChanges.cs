using System.Text.Json.Serialization;
using FW.Core.Commands;
using FW.Core.MongoDB;
using MediatR;
using MongoDB.Driver;

namespace Store.Lookup.Attributes;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AttributeType
{
    Text,
    Number,
    Decimal
}

public record CreateAttribute(Guid Id, string Name, AttributeType Type, string Unit, LookupStatus Status) : ICommand
{
    public static CreateAttribute Create(Guid id, string name, AttributeType type, string unit, LookupStatus status) =>
        new(id, name, type, unit, status);
}

public record UpdateAttribute(Guid Id, string Name, AttributeType Type, string Unit) : ICommand
{
    public static UpdateAttribute Create(Guid id, string name, AttributeType type, string unit) =>
        new(id, name, type, unit);
}

public record DeleteAttribute(Guid Id) : ICommand
{
    public static DeleteAttribute Create(Guid id) => new(id);
}

internal class HandleAttributeChanges :
    ICommandHandler<CreateAttribute>,
    ICommandHandler<UpdateAttribute>,
    ICommandHandler<DeleteAttribute>
{
    private readonly IMongoCollection<Attribute> collection;

    public HandleAttributeChanges(IMongoDatabase database)
    {
        var collectionName = MongoHelper.GetCollectionName<Attribute>();
        collection = database.GetCollection<Attribute>(collectionName);
    }

    public async Task<Unit> Handle(CreateAttribute request, CancellationToken cancellationToken)
    {
        var (id, name, type, unit, status) = request;

        var data = new Attribute
        {
            Id = id,
            Name = name,
            Type = type,
            Unit = unit,
            Status = status
        };

        await collection.InsertOneAsync(data, default, cancellationToken);

        return Unit.Value;
    }

    public async Task<Unit> Handle(UpdateAttribute request, CancellationToken cancellationToken)
    {
        var (id, name, type, unit) = request;

        var update = Builders<Attribute>.Update
            .Set(e => e.Name, name)
            .Set(e => e.Type, type)
            .Set(e => e.Unit, unit);

        await collection.UpdateOneAsync(e => e.Id == id, update, new UpdateOptions { IsUpsert = true }, cancellationToken);

        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteAttribute request, CancellationToken cancellationToken)
    {
        await collection.DeleteOneAsync(e => e.Id == request.Id, cancellationToken);

        return Unit.Value;
    }
}