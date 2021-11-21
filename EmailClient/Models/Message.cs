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
    public class Message
    {
        /// <summary>
        /// The unique ID of the message.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The subject or header of the message.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The body or content of the message.
        /// </summary>
        public string Body { get; set; }
    }
}
