using Microsoft.Extensions.DependencyInjection;

namespace FW.Core.Events;

public static class Config
{
    public static IServiceCollection AddEventHandler<TEvent, TEventHandler>(
        this IServiceCollection services
    )
        where TEventHandler : class, IEventHandler<TEvent>
    {
        return services
            .AddTransient<TEventHandler>()
            .AddTransient<IEventHandler<TEvent>>(sp => sp.GetRequiredService<TEventHandler>());
    }
}
