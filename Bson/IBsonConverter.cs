using MongoDB.Bson;
using Newtonsoft.Json;

namespace Blackwater.Core.Bson
{
    public interface IBsonConverter
    {
        BsonDocument ReadJson(JsonReader reader, Type objectType, BsonDocument? existingValue, bool hasExistingValue, JsonSerializer serializer);
        void WriteJson(JsonWriter writer, BsonDocument? value, JsonSerializer serializer);
    }
}