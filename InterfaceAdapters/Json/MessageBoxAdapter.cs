using Domain.Messages;
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
                // Serialize the account data.
                JsonDocument messageBoxJson = JsonSerializer.SerializeToDocument(messageBox);

                // Add any data assoicated with the message box.

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
    }
}
