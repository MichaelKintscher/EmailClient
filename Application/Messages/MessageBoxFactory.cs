using Domain.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages
{
    /// <summary>
    /// Factory class for creating message boxes.
    /// </summary>
    public class MessageBoxFactory
    {
        /// <summary>
        /// Creates a new message box with a new unique ID.
        /// </summary>
        /// <param name="name">The name to give the new message box.</param>
        /// <returns></returns>
        public static MessageBox CreateNewBox(string name)
        {
            // Create a new instance of the message box class.
            MessageBox messageBox = new MessageBox()
            {
                // Assign a new GUID as the ID and assign the given name.
                ID = Guid.NewGuid().ToString(),
                Name = name
            };

            return messageBox;
        }
    }
}
