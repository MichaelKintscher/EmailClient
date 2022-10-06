using Network.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApp.EventArguments
{
    /// <summary>
    /// Contains event info for a request to change a service's connection.
    /// </summary>
    internal class ChangeAccountConnectionRequestedEventArgs
    {
        /// <summary>
        /// The ID of the account associated with the connection change request. Null for new account connections.
        /// </summary>
        public string AccoutId { get; private set; }
        /// <summary>
        /// The email service provider for the account.
        /// </summary>
        public EmailProvider emailProvider { get; set; }
        /// <summary>
        /// The connection change requested.
        /// </summary>
        public ConnectionAction Action { get; private set; }

        public ChangeAccountConnectionRequestedEventArgs(string accountId, EmailProvider emailProvider, ConnectionAction action)
        {
            this.AccoutId = accountId;
            this.emailProvider = emailProvider;
            this.Action = action;
        }
    }

    public enum ConnectionAction
    {
        Connect,
        RetryConnect,
        Disconnect
    }
}
