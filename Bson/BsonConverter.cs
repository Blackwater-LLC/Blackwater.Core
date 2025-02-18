using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Blackwater.Core.Bson
{
    public class BsonConverter : JsonConverter<BsonDocument>, IBsonConverter
    {
        /// <summary>
        /// Seralize Bson -> JSON
        /// </summary>
        /// <param name="writer">The JsonWriter to write the JSON data</param>
        /// <param name="value">The BsonDocument to be serialized</param>
        /// <param name="serializer">The JsonSerializer instance calling this method</param>

        // For the future it would be preferable to have a generic BsonDocument cast to support schemas and custom collection document types
        public override void WriteJson(JsonWriter writer, BsonDocument? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            try
            {
                var json = value.ToJson();
                var jObject = JObject.Parse(json);

                jObject.WriteTo(writer);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Error while serializing", ex);
            }
        }

        /// <summary>
        /// Deserialize Json -> BSON
        /// </summary>
        /// <param name="reader">The JsonReader containing the JSON data</param>
        /// <param name="objectType">The type of object to deserialize (should be BsonDocument)</param>
        /// <param name="existingValue">Any existing BsonDocument instance (if applicable)</param>
        /// <param name="hasExistingValue">Indicates if there is an existing value</param>
        /// <param name="serializer">The JsonSerializer instance calling this method</param>
        /// <returns>A BsonDocument representing the parsed JSON data</returns>

        // for the future, same type safety support as serializing method
        public override BsonDocument ReadJson(JsonReader reader, Type objectType, BsonDocument? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                var jObject = JObject.Load(reader);
                string jsonString = jObject.ToString();

                var bsonDoc = BsonDocument.Parse(jsonString);
                return bsonDoc;
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Error while deserializing", ex);
            }
        }
    }
}
