using Domain.Messages.Emails;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApp.Controls.EventArguments
{
    /// <summary>
    /// Contains event info for a message drop event.
    /// </summary>
    public class MessageDropCompletedEventArgs
    {
        /// <summary>
        /// The emails that were dropped.
        /// </summary>
        public List<Email> Emails { get; private set; }

        /// <summary>
        /// The position in the message list the insertion happened at.
        /// </summary>
        public int InsertionIndex { get; private set; }

        /// <summary>
        /// The drag event args from the underlying drag operation.
        /// </summary>
        public DragEventArgs DragEventArgs { get; private set; }

        public MessageDropCompletedEventArgs(List<Email> emails, int insertionIndex, DragEventArgs dragEventArgs)
        {
            this.Emails = new List<Email>(emails);
            this.InsertionIndex = insertionIndex;
            this.DragEventArgs = dragEventArgs;
        }
    }
}
