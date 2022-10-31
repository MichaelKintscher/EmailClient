using Domain.Messages;
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
    public static class MessageBoxAdapter
    {
        public static string Serialize(List<MessageBox> messageBoxes)
        {
            // There are no message boxes to serialize.
            if (messageBoxes == null ||
                messageBoxes.Count < 1)
            {
                return string.Empty;
            }

            // For each message box in the list of message boxes...
            JsonArray messageBoxesArray = new JsonArray();
            foreach (MessageBox messageBox in messageBoxes)
            {
                // Serialize the message box data.
                JsonNode messageBoxJson = JsonSerializer.SerializeToNode(messageBox);

                // Add a list of the message IDs assoicated with the message box.
                JsonArray messageIds = new JsonArray();
                foreach (Email message in messageBox.Messages)
                {
                    messageIds.Add(message.ID);
                }
                messageBoxJson["MessageIDs"] = messageIds;

                // Add the message box to array.
                messageBoxesArray.Add(messageBoxJson);
            }

            // Store the array in a json object.
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("boxes", messageBoxesArray);

            return jsonObject.ToJsonString();
        }

        public static List<MessageBox> Deserialize(string lines)
        {
            JsonNode? jsonObject = JsonNode.Parse(lines);
            if (jsonObject == null)
            {
                throw new JsonFormatException("JSON to deserialize Message Box list from was not in the expected format.");
            }

            JsonArray messageBoxesArray;
            try
            {
                messageBoxesArray = jsonObject["boxes"].AsArray();
            }
            catch (Exception ex)
            {
                throw new JsonFormatException("Error was encountered when parsing the JSON.", ex);
            }

            // For each message box in the array...
            List<MessageBox> messageBoxes = new List<MessageBox>();
            foreach (var messageBoxJsonValue in messageBoxesArray)
            {
                // This is necessary because of the type iterated over in the JsonArray.
                string messageBoxJson = messageBoxJsonValue.ToJsonString();

                // Parse the messsage box data and add it to the list.
                MessageBox? messageBox = JsonSerializer.Deserialize<MessageBox>(messageBoxJson);
                if (messageBox == null)
                {
                    throw new JsonFormatException("JSON to deserialize Message Box from was not in the expected format.");
                }
                messageBoxes.Add(messageBox);
            }

            return messageBoxes;
        }

        /// <summary>
        /// Gets a list of the IDs for the messages in the given message box.
        /// </summary>
        /// <param name="boxId">The ID of the message box.</param>
        /// <param name="lines">The Json of the serialized message boxes.</param>
        /// <returns></returns>
        /// <exception cref="JsonFormatException"></exception>
        /// <exception cref="ArgumentException">Raised if the Json does not contain the given ID.</exception>
        public static List<string> GetMessageIdsInBox(string boxId, string lines)
        {
            JsonNode? jsonObject = JsonNode.Parse(lines);
            if (jsonObject == null)
            {
                throw new JsonFormatException("JSON to deserialize Message Box list from was not in the expected format.");
            }

            JsonArray messageBoxesArray;
            try
            {
                messageBoxesArray = jsonObject["boxes"].AsArray();
            }
            catch (Exception ex)
            {
                throw new JsonFormatException("Error was encountered when parsing the JSON.", ex);
            }

            // Find the message box with the matching ID.
            JsonNode? messageBoxJson = messageBoxesArray.Where(jsonNode => jsonNode["ID"].ToString() == boxId).FirstOrDefault();

            // Ensure a matching message box was found.
            if (messageBoxJson == null)
            {
                throw new ArgumentException("The given json does not contain a message box with the given ID.");
            }

            JsonArray messageIdsArray;
            try
            {
                messageIdsArray = messageBoxJson["MessageIDs"].AsArray();
            }
            catch (Exception ex)
            {
                throw new JsonFormatException("Error was encountered when parsing the JSON.", ex);
            }

            // Get the list of message IDs from the Json.
            List<string> messageIds = new List<string>();
            foreach (var messageIdValue in messageIdsArray)
            {
                string messageId = messageIdValue.ToString();
                messageIds.Add(messageId);
            }

            return messageIds;
        }
    }
}
