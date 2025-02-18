using MongoDB.Driver;

namespace Blackwater.Core.Database
{
    public class DatabaseService(Dictionary<string, MongoClient> clients) : IDatabaseService
    {

        // just a wrapper around the MongoDB.Driver APIs making it very very easy to define and inject collections on a controller level
        // the clientKeys are derived from whatever storage / store is defined under service initialization, designed to be used like:

        //        services.AddSingleton<DatabaseService>(serviceProvider =>
        //            {
        //                var mongoDBSettings = Configuration.GetSection("MDB_Settings:Databases").Get<Dictionary<string, string>>() ?? [];
        //        var clients = new Dictionary<string, MongoClient>();
        //                foreach (var setting in mongoDBSettings)
        //                {
        //                    clients.Add(setting.Key, new MongoClient(setting.Value));
        //                }
        //                return new DatabaseService(clients);
        //});

        // for prod use move away from IConfiguration / appsettings.json to a key vault

        public IMongoDatabase GetDatabase(string clientKey, string databaseName)
        {
            if (clients.TryGetValue(clientKey, out MongoClient? value))
            {
                return value.GetDatabase(databaseName);
            }
            throw new KeyNotFoundException($"MongoClient for key '{clientKey}' not found.");
        }
    }
}
