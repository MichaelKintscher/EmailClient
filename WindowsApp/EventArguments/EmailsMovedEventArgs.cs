using Domain.Messages;
using Domain.Messages.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApp.EventArguments
{
    /// <summary>
    /// Contains event info for when emails are moved between message boxes.
    /// </summary>
    internal class EmailsMovedEventArgs : EventArgs
    {
        /// <summary>
        /// The emails that moved.
        /// </summary>
        public List<Email> Emails { get; set; }

        /// <summary>
        /// The message box the email was moved from.
        /// </summary>
        public MessageBox Source { get; set; }

        /// <summary>
        /// The message box the email was moved to.
        /// </summary>
        public MessageBox Destination { get; set; }

        public EmailsMovedEventArgs(List<Email> emails, MessageBox source, MessageBox destination)
        {
            this.Emails = emails;
            this.Source = source;
            this.Destination = destination;
        }
    }
}
