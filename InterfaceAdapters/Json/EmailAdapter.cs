using Domain.Messages.Emails;
using InterfaceAdapters.Json.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace InterfaceAdapters.Json
{
    public static class EmailAdapter
    {
        /// <summary>
        /// Serializes the given email to JSON.
        /// </summary>
        /// <param name="email">The email to serialize.</param>
        /// <returns></returns>
        public static string Serialize(Email email)
        {
            return JsonSerializer.Serialize(email);
        }

        /// <summary>
        /// Deserializes the given email from JSON.
        /// </summary>
        /// <param name="emailString">The string containing the email to deserialize.</param>
        /// <returns></returns>
        public static Email Deserialize(string emailString)
        {
            return JsonSerializer.Deserialize<Email>(emailString);
        }

        /// <summary>
        /// Serializes a list of emails.
        /// </summary>
        /// <param name="emails">The list of emails to serialize.</param>
        /// <returns></returns>
        public static string SerializeEmailList(List<Email> emails)
        {
            // There are no emails to serialize.
            if (emails == null ||
                emails.Count < 1)
            {
                return string.Empty;
            }

            // For each email in the list of emails...
            JsonArray emailsArray = new JsonArray();
            foreach (Email email in emails)
            {
                // Serialize the email data and add it to the array.
                JsonDocument emailJson = JsonSerializer.SerializeToDocument(email);
                emailsArray.Add(emailJson);
            }

            // Store the array in a json object.
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("emails", emailsArray);

            return jsonObject.ToJsonString();
        }

        /// <summary>
        /// Deserializes a list of emails.
        /// </summary>
        /// <param name="emailListString">The string containing the email list to deserialize.</param>
        /// <returns></returns>
        public static List<Email> DeserializeEmailList(string emailListString)
        {
            JsonNode? jsonObject = JsonNode.Parse(emailListString);
            if (jsonObject == null)
            {
                throw new JsonFormatException("JSON to deserialize Email list from was not in the expected format.");
            }

            JsonArray emailsArray;
            try
            {
                emailsArray = jsonObject["emails"].AsArray();
            }
            catch (Exception ex)
            {
                throw new JsonFormatException("Error was encountered when parsing the JSON.", ex);
            }

            // For each email in the array...
            List<Email> emails = new List<Email>();
            foreach (var emailJsonValue in emailsArray)
            {
                // This is necessary because of the type iterated over in the JsonArray.
                string emailJson = emailJsonValue.ToJsonString();

                // Parse the email data and add it to the list.
                Email? email = JsonSerializer.Deserialize<Email>(emailJson);
                if (email == null)
                {
                    throw new JsonFormatException("JSON to deserialize Email from was not in the expected format.");
                }
                emails.Add(email);
            }

            return emails;
        }
    }
}
