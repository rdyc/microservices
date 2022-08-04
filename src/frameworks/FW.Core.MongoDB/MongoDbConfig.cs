namespace FW.Core.MongoDB
{
    public interface IMongoDbConfig
    {
        string DatabaseName { get; }
        string ConnectionString { get; }
    }

    public class MongoDbConfig : IMongoDbConfig
    {
        public string DatabaseName { get; set; } = default!;
        public string ConnectionString { get; set; } = default!;
    }
}