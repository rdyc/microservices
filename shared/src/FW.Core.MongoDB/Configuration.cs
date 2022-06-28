using FW.Core.Events;
using FW.Core.EventStoreDB;
using FW.Core.MongoDB.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FW.Core.MongoDB;

public static class Configuration
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration) =>
        services
            // .Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)).Value)
            .AddSingleton<IMongoDbSettings>(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value)
            .AddSingleton(ctx => {
                var option = ctx.GetRequiredService<IMongoDbSettings>(); 
                return new MongoClient(connectionString: option.ConnectionString);
            })
            .AddSingleton(ctx => {
                var option = ctx.GetRequiredService<IMongoDbSettings>(); 
                var client = ctx.GetRequiredService<MongoClient>();
                return client.GetDatabase(option.DatabaseName);
            })
            .AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddEventBus()
            .AddEventStoreDB(configuration);
}