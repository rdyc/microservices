using FW.Core.Events;
using FW.Core.MongoDB.Queries;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FW.Core.MongoDB.Projections;

public static class DocumentProjection
{
    public static IServiceCollection For<TView, TDbContext>(
        this IServiceCollection services,
        Action<DocumentProjectionBuilder<TView, TDbContext>> setup
    )
        where TView : IDocument
        where TDbContext : IMongoDatabase
    {
        setup(new DocumentProjectionBuilder<TView, TDbContext>(services));
        return services;
    }
}

public class DocumentProjectionBuilder<TView, TDbContext>
    where TView : IDocument
    where TDbContext : IMongoDatabase
{
    public readonly IServiceCollection services;

    public DocumentProjectionBuilder(IServiceCollection services)
    {
        this.services = services;
    }

    public DocumentProjectionBuilder<TView, TDbContext> AddOn<TEvent>(Func<EventEnvelope<TEvent>, TView> handler) where TEvent : notnull
    {
        services.AddSingleton(handler);
        services.AddTransient<IEventHandler<EventEnvelope<TEvent>>, AddProjection<TView, TEvent, TDbContext>>();

        return this;
    }

    public DocumentProjectionBuilder<TView, TDbContext> UpdateOn<TEvent>(
        Func<TEvent, object> getViewId,
        Action<EventEnvelope<TEvent>, TView> handler,
        Func<TView, CancellationToken, Task>? prepare = null
    ) where TEvent : notnull
    {
        services.AddSingleton(getViewId);
        services.AddSingleton(handler);
        services.AddTransient<IEventHandler<EventEnvelope<TEvent>>, UpdateProjection<TView, TEvent, TDbContext>>();

        if (prepare != null)
        {
            services.AddSingleton(prepare);
        }

        return this;
    }

    public DocumentProjectionBuilder<TView, TDbContext> QueryWith<TQuery>(
        Func<IQueryable<TView>, TQuery, CancellationToken, Task<TView>> handler
    )
    {
        services.AddEntityFrameworkQueryHandler<TDbContext, TQuery, TView>(handler);

        return this;
    }

    public DocumentProjectionBuilder<TView, TDbContext> QueryWith<TQuery>(
        Func<IQueryable<TView>, TQuery, CancellationToken, Task<IReadOnlyList<TView>>> handler
    )
    {
        services.AddEntityFrameworkQueryHandler<TDbContext, TQuery, TView>(handler);

        return this;
    }
}

public class AddProjection<TView, TEvent, TDbContext> : IEventHandler<EventEnvelope<TEvent>>
    where TView : class
    where TDbContext : IMongoDatabase
    where TEvent : notnull
{
    private readonly TDbContext dbContext;
    private readonly Func<EventEnvelope<TEvent>, TView> create;

    public AddProjection(
        TDbContext dbContext,
        Func<EventEnvelope<TEvent>, TView> create
    )
    {
        this.dbContext = dbContext;
        this.create = create;
    }

    public async Task Handle(EventEnvelope<TEvent> eventEnvelope, CancellationToken ct)
    {
        var view = create(eventEnvelope);

        await dbContext.AddAsync(view, ct);
        await dbContext.SaveChangesAsync(ct);
    }
}

public class UpdateProjection<TView, TEvent, TDbContext> : IEventHandler<EventEnvelope<TEvent>>
    where TView : IDocument
    where TDbContext : IMongoCollection<TDbContext>
    where TEvent : notnull
{
    private readonly TDbContext dbContext;
    private readonly Func<TEvent, object> getViewId;
    private readonly Action<EventEnvelope<TEvent>, TView> update;
    private readonly Func<TView, CancellationToken, Task>? prepare;

    public UpdateProjection(
        TDbContext dbContext,
        Func<TEvent, object> getViewId,
        Action<EventEnvelope<TEvent>, TView> update,
        Func<TView, CancellationToken, Task>? prepare = null)
    {
        this.dbContext = dbContext;
        this.getViewId = getViewId;
        this.update = update;
        this.prepare = prepare;
    }

    public async Task Handle(EventEnvelope<TEvent> eventEnvelope, CancellationToken ct)
    {
        var viewId = getViewId(eventEnvelope.Data);
        var view = await dbContext.FindAsync<TView>(new[] { viewId }, ct);

        if (view == null)
            throw new InvalidOperationException($"{typeof(TView).Name} with id {viewId} wasn't found");

        prepare?.Invoke(dbContext.GetCollection(view), ct);

        update(eventEnvelope, view);

        await dbContext.SaveChangesAsync(ct);
    }
}
