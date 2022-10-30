using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApp.EventArguments
{
    /// <summary>
    /// Contains event info for a request to create a new message box.
    /// </summary>
    internal class CreateMessageBoxRequestedEventArgs : EventArgs
    {
        /// <summary>
        /// The name for the new message box.
        /// </summary>
        public string Name { get; set; }

        public CreateMessageBoxRequestedEventArgs(string name)
        {
            this.Name = name;
        }
    }
}
