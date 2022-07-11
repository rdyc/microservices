using EventStore.Client;
using FW.Core.EventStoreDB;
using FW.Core.EventStoreDB.Subscriptions;
using FW.Core.MongoDB;
using FW.Core.MongoDB.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Validator.Currencies;

namespace Validator;

public static class ValidatorServices
{
    public static IServiceCollection AddValidatorServices(this IServiceCollection services, IConfiguration configuration) =>
        services
            .Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)))
            .AddMongoDb(configuration)
            .AddEventStoreDB(configuration)
            .AddLookup();
}