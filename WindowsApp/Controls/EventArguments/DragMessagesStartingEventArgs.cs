using Domain.Messages.Emails;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApp.Controls.EventArguments
{
    /// <summary>
    /// Contains event info for a message drag event.
    /// </summary>
    public class DragMessagesStartingEventArgs : EventArgs
    {
        /// <summary>
        /// The emails being dragged.
        /// </summary>
        public List<Email> Emails { get; private set; }

        /// <summary>
        /// The drag event args from the underlying drag operation.
        /// </summary>
        public DragItemsStartingEventArgs DragItemsStartingEventArgs { get; private set; }

        public DragMessagesStartingEventArgs(List<Email> emails, DragItemsStartingEventArgs dragItemsStartingEventArgs)
        {
            this.Emails = new List<Email>(emails);
            this.DragItemsStartingEventArgs = dragItemsStartingEventArgs;
        }
    }
}
