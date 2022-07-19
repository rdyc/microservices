using EventStore.Client;
using FW.Core.EventStoreDB;
using FW.Core.EventStoreDB.Subscriptions;
using FW.Core.Validation;
using FW.Core.MongoDB;
using Lookup.Currencies;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Lookup.Attributes;
using Lookup.Histories;

namespace Lookup;

public static class LookupServices
{
    public static IServiceCollection AddLookupServices(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>))
            .AddScoped(typeof(IRequestPreProcessor<>), typeof(GenericRequestPreProcessor<>))
            .AddScoped(typeof(IRequestPostProcessor<,>), typeof(GenericRequestPostProcessor<,>))
            .AddMongoDb(configuration)
            .AddEventStoreDB(configuration)
            .AddEventStoreDBSubscriptionToAll(new EventStoreDBSubscriptionToAllOptions
            {
                SubscriptionId = "lookup",
                FilterOptions = new(EventTypeFilter.RegularExpression(@"Attribute|Currency"))
            })
            .AddCurrency()
            .AddAttribute()
            .AddHistory();
}