using Domain.Messages;
using Domain.Messages.Emails;
using InterfaceAdapters.Json.Common;
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
    /// <summary>
    /// Converts a list of message boxes to/from JSON.
    /// </summary>
    public static class MessageBoxAdapter
    {
        /// <summary>
        /// The property name to give the serialized JSON list on the root JSON object.
        /// </summary>
        private static readonly string RootPropertyName = "boxes";

        /// <summary>
        /// Serializes the list of message boxes to JSON.
        /// </summary>
        /// <param name="messageBoxes">The list of message boxes to serialize.</param>
        /// <returns></returns>
        public static string Serialize(List<MessageBox> messageBoxes)
        {
            return EntityListAdapter<MessageBox>.Serialize(messageBoxes, MessageBoxAdapter.RootPropertyName);
        }

        /// <summary>
        /// Deserializes a list of message boxes from JSON.
        /// </summary>
        /// <param name="lines">The JSON to deserialize from.</param>
        /// <returns></returns>
        public static List<MessageBox> Deserialize(string lines)
        {
            return EntityListAdapter<MessageBox>.Deserialize(lines, MessageBoxAdapter.RootPropertyName);
        }
    }
}
