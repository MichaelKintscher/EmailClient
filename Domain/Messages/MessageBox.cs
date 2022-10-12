using Domain.Messages.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Messages
{
    /// <summary>
    /// Model for a box of messages.
    /// </summary>
    public class MessageBox
    {
        /// <summary>
        /// The unique ID of the message box.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The name of the message box.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of messages in the message box.
        /// </summary>
        public List<Email> Messages { get; set; }

        /// <summary>
        /// Default constructor - creates a message box with an empty string name and empty list of messages.
        /// </summary>
        public MessageBox()
        {
            this.Name = string.Empty;
            this.Messages = new List<Email>();
        }
    }
}
