using FW.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Validator.Lookup;

namespace Validator.Currencies;

internal static class LookupService
{
    public static IServiceCollection AddLookup(this IServiceCollection services) =>
        services.AddEventHandlers();

    private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
        services
            .AddEventHandler<EventEnvelope<LookupChanged>, HandleLookupChanged>();
}