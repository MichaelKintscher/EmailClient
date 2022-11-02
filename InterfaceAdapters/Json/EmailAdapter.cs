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
    /// Converts a list of emails to/from JSON.
    /// </summary>
    public static class EmailAdapter
    {
        /// <summary>
        /// The property name to give the serialized JSON list on the root JSON object.
        /// </summary>
        private static readonly string RootPropertyName = "messages";

        /// <summary>
        /// Serializes the given list of emails to JSON.
        /// </summary>
        /// <param name="emails">The list of emails to serialize.</param>
        /// <returns></returns>
        public static string Serialize(List<Email> emails)
        {
            return EntityListAdapter<Email>.Serialize(emails, EmailAdapter.RootPropertyName);
        }

        /// <summary>
        /// Deserializes the given email from JSON.
        /// </summary>
        /// <param name="lines">The string containing the email to deserialize.</param>
        /// <returns></returns>
        public static List<Email> Deserialize(string lines)
        {
            return EntityListAdapter<Email>.Deserialize(lines, EmailAdapter.RootPropertyName);
        }
    }
}
