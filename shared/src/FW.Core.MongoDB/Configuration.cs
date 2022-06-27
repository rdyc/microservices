using FW.Core.Events;
using FW.Core.EventStoreDB;
using FW.Core.MongoDB.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FW.Core.MongoDB;

public static class Configuration
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"))
            .AddSingleton<IMongoDbSettings>(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value)
            .AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuratio) =>
        services
            .AddEventBus()
            .AddEventStoreDB(configuration);
}