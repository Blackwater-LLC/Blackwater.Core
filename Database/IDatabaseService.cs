using MongoDB.Driver;

namespace Blackwater.Core.Database
{
    public interface IDatabaseService
    {
        IMongoDatabase GetDatabase(string clientKey, string databaseName);
    }
}