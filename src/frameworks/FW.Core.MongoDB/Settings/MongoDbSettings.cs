namespace FW.Core.MongoDB.Settings
{
    public interface IMongoDbSettings
    {
        string DatabaseName { get; }
        string ConnectionString { get; }
    }

    public class MongoDbSettings : IMongoDbSettings
    {
        public string DatabaseName { get; set; } = default!;
        public string ConnectionString { get; set; } = default!;
    }
}