using FW.Core.Commands;
using FW.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Store.Lookup.Attributes;
using Store.Lookup.Currencies;

namespace Store.Lookup;

internal static class LookupServices
{
    public static IServiceCollection AddLookup(this IServiceCollection services) =>
        services.AddEventHandlers()
            .AddCommandHandlers();

    private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddEventHandler<EventEnvelope<LookupChanged>, HandleLookupChanged>();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<CreateAttribute, HandleAttributeChanges>()
            .AddCommandHandler<UpdateAttribute, HandleAttributeChanges>()
            .AddCommandHandler<DeleteAttribute, HandleAttributeChanges>()
            .AddCommandHandler<CreateCurrency, HandleCurrencyChanges>()
            .AddCommandHandler<UpdateCurrency, HandleCurrencyChanges>()
            .AddCommandHandler<DeleteCurrency, HandleCurrencyChanges>();
}