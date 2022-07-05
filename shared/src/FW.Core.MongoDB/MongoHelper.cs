namespace FW.Core.MongoDB;

public static class MongoHelper
{
    public static string GetCollectionName<T>()
    {
        var type = typeof(T);
        var attribute = type.GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault() as BsonCollectionAttribute;

        if (attribute != null)
        {
            return attribute.CollectionName;
        }
        else
        {
            return type.Name;
        }
    }
}