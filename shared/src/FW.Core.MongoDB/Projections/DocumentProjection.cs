using FW.Core.Events;
using FW.Core.MongoDB.Queries;
using FW.Core.MongoDB.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FW.Core.MongoDB.Projections;

public static class DocumentProjection
{
    public static IServiceCollection For<TView>(this IServiceCollection services, Action<DocumentProjectionBuilder<TView>> setup)
        where TView : IDocument
    {
        setup(new DocumentProjectionBuilder<TView>(services));
        return services;
    }
}

public class DocumentProjectionBuilder<TView>
    where TView : IDocument
{
    public readonly IServiceCollection services;

    public DocumentProjectionBuilder(IServiceCollection services)
    {
        this.services = services;
    }

    public DocumentProjectionBuilder<TView> AddOn<TEvent>(Func<EventEnvelope<TEvent>, TView> handler) where TEvent : notnull
    {
        services.AddSingleton(handler);
        services.AddTransient<IEventHandler<EventEnvelope<TEvent>>, AddProjection<TView, TEvent>>();

        return this;
    }

    public DocumentProjectionBuilder<TView> UpdateOn<TEvent>(Func<TEvent, Guid> getViewId, Func<TView, UpdateDefinition<TView>> prepare, Action<EventEnvelope<TEvent>, TView> handler) where TEvent : notnull
    {
        services.AddSingleton(getViewId);
        services.AddSingleton(handler);
        services.AddSingleton(prepare);
        services.AddTransient<IEventHandler<EventEnvelope<TEvent>>, UpdateProjection<TView, TEvent>>();

        return this;
    }

    /* public DocumentProjectionBuilder<TView, TDbContext> QueryWith<TQuery>(
        Func<IQueryable<TView>, TQuery, CancellationToken, Task<TView>> handler
    )
    {
        services.AddEntityFrameworkQueryHandler<TDbContext, TQuery, TView>(handler);

        return this;
    } */

    /* public DocumentProjectionBuilder<TView, TDbContext> QueryWith<TQuery>(
        Func<IQueryable<TView>, TQuery, CancellationToken, Task<IReadOnlyList<TView>>> handler
    )
    {
        services.AddEntityFrameworkQueryHandler<TDbContext, TQuery, TView>(handler);

        return this;
    } */
}

public class AddProjection<TView, TEvent> : IEventHandler<EventEnvelope<TEvent>>
    where TView : IDocument
    where TEvent : notnull
{
    private readonly IMongoCollection<TView> collection;
    private readonly Func<EventEnvelope<TEvent>, TView> create;

    public AddProjection(IMongoDatabase mongoDb, Func<EventEnvelope<TEvent>, TView> create)
    {
        this.collection = mongoDb.GetCollection<TView>("currency_shortinfo");
        this.create = create;
    }

    public async Task Handle(EventEnvelope<TEvent> eventEnvelope, CancellationToken ct)
    {
        var view = create(eventEnvelope);

        await collection.InsertOneAsync(view, null, ct);
    }
}

public class UpdateProjection<TView, TEvent> : IEventHandler<EventEnvelope<TEvent>>
    where TView : IDocument
    where TEvent : notnull
{
    private readonly IMongoCollection<TView> collection;
    private readonly Func<TEvent, Guid> getViewId;
    private readonly Func<TView, UpdateDefinition<TView>> prepare;
    private readonly Action<EventEnvelope<TEvent>, TView> handler;

    public UpdateProjection(
        IMongoDatabase mongoDb,
        Func<TEvent, Guid> getViewId,
        Func<TView, UpdateDefinition<TView>> prepare,
        Action<EventEnvelope<TEvent>, TView> handler)
    {
        this.collection = mongoDb.GetCollection<TView>("currency_shortinfo");
        this.getViewId = getViewId;
        this.handler = handler;
        this.prepare = prepare;
    }

    public async Task Handle(EventEnvelope<TEvent> eventEnvelope, CancellationToken ct)
    {
        var viewId = getViewId(eventEnvelope.Data);
        var filter = Builders<TView>.Filter.Eq(doc => doc.Id, viewId);
        var view = await collection.Find(filter).SingleOrDefaultAsync(ct);

        if (view == null)
            throw new InvalidOperationException($"{typeof(TView).Name} with id {viewId} wasn't found");

        handler(eventEnvelope, view);

        var updateDefinition = prepare.Invoke(view);

        await collection.UpdateOneAsync<TView>(e => e.Id == viewId, updateDefinition, new UpdateOptions() { IsUpsert = true }, ct);
    }
}