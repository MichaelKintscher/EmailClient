using InterfaceAdapters.Json.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace InterfaceAdapters.Json.Common
{
    /// <summary>
    /// Converts a list of an entity to/from JSON with shallow serialization.
    /// </summary>
    public static class EntityListAdapter<T>
    {
        /// <summary>
        /// Serializes a list of entities to JSON. The list is converted to a JSON
        /// array of JSON objects. The array itself is stored as a property on the
        /// root JSON object with the given property name.
        /// </summary>
        /// <param name="entities">The list of entities to serialize.</param>
        /// <param name="propertyName">The property name to give the serialized JSON list on the root JSON object.</param>
        /// <returns></returns>
        public static string Serialize(List<T> entities, string propertyName)
        {
            // There are no entities to serialize.
            if (entities == null ||
                entities.Count < 1)
            {
                return string.Empty;
            }

            // For each entity in the list...
            JsonArray entitiesJsonArray = new JsonArray();
            foreach (T entity in entities)
            {
                // Serialize the entity data and add it to the array.
                JsonDocument entityJson = JsonSerializer.SerializeToDocument(entity);
                entitiesJsonArray.Add(entityJson);
            }

            // Store the array in a json object.
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add(propertyName, entitiesJsonArray);

            return jsonObject.ToJsonString();
        }

        /// <summary>
        /// Deserializes a list of entities from JSON.
        /// </summary>
        /// <param name="lines">The JSON to deserialize from. The list of entities should be stored on a property of the root JSON object as a JSON array of JSON objects.</param>
        /// <param name="propertyName">The name of the property on the root JSON object the JSON array is stored on.</param>
        /// <returns></returns>
        /// <exception cref="JsonFormatException">Thrown if the given JSON is not in the correct format.</exception>
        public static List<T> Deserialize(string lines, string propertyName)
        {
            // Validate the JSON format.
            JsonNode? jsonObject = JsonNode.Parse(lines);
            if (jsonObject == null)
            {
                throw new JsonFormatException("JSON to deserialize " + typeof(T).Name + " list from was not in the expected format.");
            }

            JsonArray entityJsonArray;
            try
            {
                entityJsonArray = jsonObject[propertyName].AsArray();
            }
            catch (Exception ex)
            {
                throw new JsonFormatException("Error was encountered when parsing the JSON.", ex);
            }

            // For each entity in the array...
            List<T> entities = new List<T>();
            foreach (var entityJsonValue in entityJsonArray)
            {
                // This is necessary because of the type iterated over in the JsonArray.
                string entityJson = entityJsonValue.ToJsonString();

                // Parse the messsage box data and add it to the list.
                T? entity = JsonSerializer.Deserialize<T>(entityJson);
                if (entity == null)
                {
                    throw new JsonFormatException("JSON to deserialize " + typeof(T).Name + " from was not in the expected format.");
                }
                entities.Add(entity);
            }

            return entities;
        }
    }
}
