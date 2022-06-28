using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FW.Core.MongoDB;

public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    Guid Id { get; set; }

    // DateTime CreatedAt { get; }
}

public abstract record Document : IDocument
{
    public Guid Id { get; set; }

    // public DateTime CreatedAt => Id.CreationTime;
}