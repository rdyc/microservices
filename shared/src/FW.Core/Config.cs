using FW.Core.Events;
using FW.Core.Tracing;
using FW.Core.Tracing.Causation;
using FW.Core.Tracing.Correlation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FW.Core;

public static class Config
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddMediatR()
            // .AddScoped<ICommandBus, CommandBus>()
            // .AddScoped<IQueryBus, QueryBus>()
            .AddTracing()
            .AddEventBus();

        // services.TryAddScoped<IExternalCommandBus, ExternalCommandBus>();

        // services.TryAddScoped<IIdGenerator, NulloIdGenerator>();

        return services;
    }

    public static IServiceCollection AddTracing(this IServiceCollection services)
    {
        services.TryAddSingleton<ICorrelationIdFactory, GuidCorrelationIdFactory>();
        services.TryAddSingleton<ICausationIdFactory, GuidCausationIdFactory>();
        services.TryAddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
        services.TryAddScoped<ICausationIdProvider, CausationIdProvider>();
        services.TryAddScoped<ITraceMetadataProvider, TraceMetadataProvider>();
        services.TryAddSingleton<ITracingScopeFactory, TracingScopeFactory>();

        services.TryAddScoped<Func<IServiceProvider, TraceMetadata?, TracingScope>>(sp =>
            (scopedServiceProvider, traceMetadata) =>
                sp.GetRequiredService<ITracingScopeFactory>().CreateTraceScope(scopedServiceProvider, traceMetadata)
        );

        services.TryAddScoped<Func<IServiceProvider, IEventEnvelope?, TracingScope>>(sp =>
            (scopedServiceProvider, eventEnvelope) => sp.GetRequiredService<ITracingScopeFactory>()
                .CreateTraceScope(scopedServiceProvider, eventEnvelope)
        );

        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        return services
            .AddScoped<IMediator, Mediator>()
            .AddTransient<ServiceFactory>(sp => sp.GetRequiredService!);
    }
}
