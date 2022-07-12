using FW.Core.Events;
using FW.Core.Projections;
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

    public static IServiceCollection Project<TEvent, TDocument>(
        this IServiceCollection services,
        Func<TEvent, Guid> getId,
        Func<Guid, FilterDefinitionBuilder<TDocument>, FilterDefinition<TDocument>> filterBy)
        where TDocument : IDocument, IVersionedProjection
        where TEvent : notnull
    {
        services.AddTransient<IEventHandler<EventEnvelope<TEvent>>>(sp =>
            {
                var collectionName = MongoHelper.GetCollectionName<TDocument>();
                var collection = sp.GetRequiredService<IMongoDatabase>()
                    .GetCollection<TDocument>(collectionName);

                return new DocumentProjection<TEvent, TDocument>(collection, getId, filterBy);
            });

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

public class DocumentProjection<TEvent, TDocument> :
    IEventHandler<EventEnvelope<TEvent>>
    where TDocument : IDocument, IVersionedProjection
    where TEvent : notnull
{
    private readonly IMongoCollection<TDocument> collection;
    private readonly Func<TEvent, Guid> getId;
    private readonly Func<Guid, FilterDefinitionBuilder<TDocument>, FilterDefinition<TDocument>> filterBy;

    public DocumentProjection(
        IMongoCollection<TDocument> collection,
        Func<TEvent, Guid> getId,
        Func<Guid, FilterDefinitionBuilder<TDocument>, FilterDefinition<TDocument>> filterBy
    )
    {
        this.collection = collection;
        this.getId = getId;
        this.filterBy = filterBy;
    }

    public async Task Handle(EventEnvelope<TEvent> eventEnvelope, CancellationToken cancellationToken)
    {
        var (@event, eventMetadata) = eventEnvelope;

        var viewId = getId(@event); 
        var filterBuilder = Builders<TDocument>.Filter;
        var filter = filterBy.Invoke(viewId, filterBuilder);
        var entity = await collection.Find(filter).SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
            throw new InvalidOperationException($"{typeof(TDocument).Name} with id {viewId} wasn't found");

        var eventLogPosition = eventMetadata.LogPosition;

        if (entity.LastProcessedPosition >= eventLogPosition)
            return;

        entity.When(@event);

        entity.Id = Guid.Parse(eventMetadata.EventId);
        entity.LastProcessedPosition = eventLogPosition;

        await collection.InsertOneAsync(entity, default, cancellationToken);
    }
}