using FW.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FW.Core.MongoDB.Projections;

public static class DocumentProjection
{
    public static IServiceCollection Projection<TDocument>(
        this IServiceCollection services,
        Action<DocumentProjectionBuilder<TDocument>> builder)
        where TDocument : IDocument
    {
        var collectionName = MongoHelper.GetCollectionName<TDocument>();

        builder(new DocumentProjectionBuilder<TDocument>(services, collectionName));

        return services;
    }
}

public class DocumentProjectionBuilder<TDocument>
    where TDocument : IDocument
{
    public readonly IServiceCollection services;
    public readonly string collectionName;

    public DocumentProjectionBuilder(
        IServiceCollection services,
        string collectionName)
    {
        this.services = services;
        this.collectionName = collectionName;
    }

    public DocumentProjectionBuilder<TDocument> AddOn<TEvent>(
        Func<EventEnvelope<TEvent>, TDocument> onHandle)
        where TEvent : notnull
    {
        services.AddTransient<IEventHandler<EventEnvelope<TEvent>>>(sp =>
        {
            var collection = sp.GetRequiredService<IMongoDatabase>()
                .GetCollection<TDocument>(collectionName);

            return new AddProjection<TDocument, TEvent>(collection, onHandle);
        });

        return this;
    }

    public DocumentProjectionBuilder<TDocument> UpdateOn<TEvent>(
        Func<TEvent, Guid> onGet,
        Func<TDocument, UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> onUpdate,
        Action<EventEnvelope<TEvent>, TDocument> onHandle
    ) where TEvent : notnull
    {
        services.AddTransient<IEventHandler<EventEnvelope<TEvent>>>(sp =>
        {
            var collection = sp.GetRequiredService<IMongoDatabase>()
                .GetCollection<TDocument>(collectionName);

            return new UpdateProjection<TDocument, TEvent>(collection, onHandle, onGet, onUpdate);
        });

        return this;
    }
}

public class AddProjection<TDocument, TEvent> :
    IEventHandler<EventEnvelope<TEvent>>
    where TDocument : IDocument
    where TEvent : notnull
{
    private readonly IMongoCollection<TDocument> collection;
    private readonly Func<EventEnvelope<TEvent>, TDocument> onCreate;

    public AddProjection(
        IMongoCollection<TDocument> collection,
        Func<EventEnvelope<TEvent>, TDocument> onCreate)
    {
        this.collection = collection;
        this.onCreate = onCreate;
    }

    public async Task Handle(EventEnvelope<TEvent> eventEnvelope, CancellationToken cancellationToken)
    {
        var view = onCreate(eventEnvelope);

        await collection.InsertOneAsync(view, default, cancellationToken);
    }
}

public class UpdateProjection<TDocument, TEvent> :
    IEventHandler<EventEnvelope<TEvent>>
    where TDocument : IDocument
    where TEvent : notnull
{
    private readonly IMongoCollection<TDocument> collection;
    private readonly Func<TEvent, Guid> onGet;
    private readonly Func<TDocument, UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> onUpdate;
    private readonly Action<EventEnvelope<TEvent>, TDocument> onHandle;

    public UpdateProjection(
        IMongoCollection<TDocument> collection,
        Action<EventEnvelope<TEvent>, TDocument> onHandle,
        Func<TEvent, Guid> onGet,
        Func<TDocument, UpdateDefinitionBuilder<TDocument>, UpdateDefinition<TDocument>> onUpdate
    )
    {
        this.collection = collection;
        this.onHandle = onHandle;
        this.onGet = onGet;
        this.onUpdate = onUpdate;
    }

    public async Task Handle(EventEnvelope<TEvent> eventEnvelope, CancellationToken cancellationToken)
    {
        var viewId = onGet(eventEnvelope.Data);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, viewId);
        var view = await collection.Find(filter).SingleOrDefaultAsync(cancellationToken);

        if (view == null)
            throw new InvalidOperationException($"{typeof(TDocument).Name} with id {viewId} wasn't found");

        onHandle(eventEnvelope, view);

        var updateBuilder = Builders<TDocument>.Update;
        var update = onUpdate.Invoke(view, updateBuilder);

        await collection.UpdateOneAsync(e => e.Id == viewId, update, default, cancellationToken);
    }
}