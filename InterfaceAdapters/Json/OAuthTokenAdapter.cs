using CleanArchitecture.Core.Adapters.Json;
using Domain.Common;
using InterfaceAdapters.Json.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InterfaceAdapters.Json
{
    public class OAuthTokenAdapter : IJsonAdapter<Dictionary<string, OAuthToken>>
    {
        /// <summary>
        /// Serializes the current token data into a JSON string.
        /// </summary>
        /// <param name="data">The token data collection to serialize.</param>
        /// <returns>A serialized JSON representation of the given token data.</returns>
        public string Serialize(Dictionary<string, OAuthToken> data)
        {
            return this.SerializeAsJsonObject(data).ToJsonString();
        }

        /// <summary>
        /// Deserializes the token data from a JSON string.
        /// </summary>
        /// <param name="json">The JSON string containing token data to deserialize.</param>
        /// <returns>The token data collection deserialized from the given JSON string.</returns>
        public Dictionary<string, OAuthToken> Deserialize(string json)
        {
            // Parse the list of accounts' token data.
            JsonNode? jsonData;
            try
            {
                jsonData = JsonNode.Parse(json);
            }
            catch (Exception ex)
            {
                throw new JsonFormatException("Error was encountered when parsing the JSON.", ex);
            }

            if (jsonData == null)
            {
                throw new JsonFormatException("JSON to deserialize token from was not in the expected format.");
            }

            JsonArray itemsArray;
            try
            {
                itemsArray = jsonData["items"].AsArray();
            }
            catch (NullReferenceException ex)
            {
                throw new JsonFormatException("JSON to deserialize token from was not in the expected format.", ex);
            }

            // Create an empty list of token data, and parse and add each account's token data.
            Dictionary<string, OAuthToken> accountsTokenData = new Dictionary<string, OAuthToken>();
            foreach (var tokenDataJsonValue in itemsArray)
            {
                // Skip any null values in the list.
                if (tokenDataJsonValue == null)
                {
                    continue;
                }

                // Parse the response json content.
                JsonObject tokenDataJson = tokenDataJsonValue.AsObject();
                string token = tokenDataJson.ContainsKey("access_token") ? tokenDataJson["access_token"]!.GetValue<string>() : "";
                long expiresInSeconds = tokenDataJson.ContainsKey("expires_in") ? (long)tokenDataJson["expires_in"]! : 0;
                string tokenType = tokenDataJson.ContainsKey("token_type") ? tokenDataJson["token_type"]!.GetValue<string>() : "";
                string scope = tokenDataJson.ContainsKey("scope") ? tokenDataJson["scope"]!.GetValue<string>() : "";
                string refreshToken = tokenDataJson.ContainsKey("refresh_token") ? tokenDataJson["refresh_token"]!.GetValue<string>() : "";

                // If the response content contains an issued time, use that. Otherwise, default to Utc now.
                DateTime issuedTime = tokenDataJson.ContainsKey("issued_time") ? DateTime.Parse(tokenDataJson["issued_time"]!.GetValue<string>()) : DateTime.UtcNow;

                // Create and return a new instance of the OAuthToken class.
                OAuthToken tokenData = new OAuthToken()
                {
                    AccessToken = token,
                    TokenType = tokenType,
                    ExpiresInSeconds = expiresInSeconds,
                    RefreshToken = refreshToken,
                    Scope = scope,
                    IdToken = "",
                    IssuedUtc = issuedTime
                };

                // Parse the key for this token data and add both to the collection.
                if (tokenDataJson["key"] == null)
                {
                    throw new JsonFormatException("JSON to deserialize token from was not in the expected format - key is missing.");
                }
                string key = tokenDataJson["key"]!.GetValue<string>();
                accountsTokenData.Add(key, tokenData);
            }

            return accountsTokenData;
        }

        public string SerializeList(List<Dictionary<string, OAuthToken>> entities, string propertyName = "values")
        {
            // There are no entities to serialize.
            if (entities == null ||
                entities.Count < 1)
            {
                return string.Empty;
            }

            // For each entity in the list...
            JsonArray entitiesJsonArray = new JsonArray();
            foreach (Dictionary<string, OAuthToken> entity in entities)
            {
                // Serialize the entity data and add it to the array.
                JsonObject entityJson = this.SerializeAsJsonObject(entity);
                entitiesJsonArray.Add(entityJson);
            }

            // Store the array in a json object.
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add(propertyName, entitiesJsonArray);

            return jsonObject.ToJsonString();
        }

        public List<Dictionary<string, OAuthToken>> DeserializeList(string json, string propertyName = "values")
        {
            // Validate the JSON format.
            JsonNode? jsonObject = JsonNode.Parse(json);
            if (jsonObject == null)
            {
                throw new JsonException("JSON to deserialize " + typeof(Dictionary<string, OAuthToken>).Name + " list from was not in the expected format.");
            }

            JsonArray entityJsonArray;
            try
            {
                entityJsonArray = jsonObject[propertyName].AsArray();
            }
            catch (Exception ex)
            {
                throw new JsonException("Error was encountered when parsing the JSON.", ex);
            }

            // For each entity in the array...
            List<Dictionary<string, OAuthToken>> entities = new List<Dictionary<string, OAuthToken>>();
            foreach (var entityJsonValue in entityJsonArray)
            {
                // This is necessary because of the type iterated over in the JsonArray.
                string entityJson = entityJsonValue.ToJsonString();

                // Parse the entity data and add it to the list.
                Dictionary<string, OAuthToken>? entity = this.Deserialize(entityJson);
                if (entity == null)
                {
                    throw new JsonException("JSON to deserialize " + typeof(Dictionary<string, OAuthToken>).Name + " from was not in the expected format.");
                }
                entities.Add(entity);
            }

            return entities;
        }

        #region Helper Methods
        private JsonObject SerializeAsJsonObject(Dictionary<string, OAuthToken> data)
        {
            // Create a JSON array for all of the accounts' token data.
            JsonArray accountsTokenDataArray = new JsonArray();

            // For each account...
            foreach (string key in data.Keys)
            {
                // Create a JSON object for the account's token. and add the key.
                JsonObject accountTokenJson = new JsonObject();
                accountTokenJson.Add("key", JsonValue.Create(key));

                accountTokenJson.Add("access_token", JsonValue.Create(data[key].AccessToken));

                // Set the expiration time (lifespan of the token) to zero seconds if no value exists.
                double expirationTime = data[key].ExpiresInSeconds.HasValue ? (double)data[key].ExpiresInSeconds.Value : 0.0;
                accountTokenJson.Add("expires_in", JsonValue.Create(expirationTime));

                accountTokenJson.Add("token_type", JsonValue.Create(data[key].TokenType));
                accountTokenJson.Add("scope", JsonValue.Create(data[key].Scope));
                accountTokenJson.Add("refresh_token", JsonValue.Create(data[key].RefreshToken));
                accountTokenJson.Add("issued_time", JsonValue.Create(data[key].IssuedUtc.ToString()));

                // Add the JSON object for the account's token to the JSON array.
                accountsTokenDataArray.Add(accountTokenJson);
            }

            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("items", accountsTokenDataArray);

            return jsonObject;
        }
        #endregion
    }
}
