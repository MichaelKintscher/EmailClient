using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailClient.Models
{
    /// <summary>
    /// Represents an email or other message.
    /// </summary>
    internal class Message
    {
        /// <summary>
        /// The unique ID of the message.
        /// </summary>
        internal Guid Id { get; set; }

        /// <summary>
        /// The subject or header of the message.
        /// </summary>
        internal string Subject { get; set; }

        /// <summary>
        /// The body or content of the message.
        /// </summary>
        internal string Body { get; set; }
    }
}
