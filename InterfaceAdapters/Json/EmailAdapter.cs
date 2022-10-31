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
        /// Serializes the given list of emails to JSON.
        /// </summary>
        /// <param name="emails">The list of emails to serialize.</param>
        /// <returns></returns>
        public static string Serialize(List<Email> emails)
        {
            // There are no messages to serialize.
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
            jsonObject.Add("messages", emailsArray);

            return jsonObject.ToJsonString();
        }

        /// <summary>
        /// Deserializes the given email from JSON.
        /// </summary>
        /// <param name="lines">The string containing the email to deserialize.</param>
        /// <returns></returns>
        public static List<Email> Deserialize(string lines)
        {
            JsonNode? jsonObject = JsonNode.Parse(lines);
            if (jsonObject == null)
            {
                throw new JsonFormatException("JSON to deserialize message list from was not in the expected format.");
            }

            JsonArray messagesArray;
            try
            {
                messagesArray = jsonObject["messages"].AsArray();
            }
            catch (Exception ex)
            {
                throw new JsonFormatException("Error was encountered when parsing the JSON.", ex);
            }

            // For each message in the array...
            List<Email> messages = new List<Email>();
            foreach (var messageJsonValue in messagesArray)
            {
                // This is necessary because of the type iterated over in the JsonArray.
                string messageJson = messageJsonValue.ToJsonString();

                // Parse the message data and add it to the list.
                Email? message = JsonSerializer.Deserialize<Email>(messageJson);
                if (message == null)
                {
                    throw new JsonFormatException("JSON to deserialize message from was not in the expected format.");
                }
                messages.Add(message);
            }

            return messages;
        }
    }
}
