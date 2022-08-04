using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FW.Core.MongoDB;

public static class Configuration
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration) =>
        services
            .Configure<MongoDbConfig>(configuration.GetSection("MongoDb"))
            .AddSingleton<IMongoDbConfig>(sp => sp.GetRequiredService<IOptions<MongoDbConfig>>().Value)
            .AddSingleton(sp => {
                var config = sp.GetRequiredService<IMongoDbConfig>(); 
                return new MongoClient(connectionString: config.ConnectionString);
            })
            .AddSingleton(sp => {
                var config = sp.GetRequiredService<IMongoDbConfig>(); 
                var client = sp.GetRequiredService<MongoClient>();
                return client.GetDatabase(config.DatabaseName);
            });
}