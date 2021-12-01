using EmailClient.Models.AppConfigModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailClient.EventArguments
{
    /// <summary>
    /// Contains event info for when an OAuth code is acquired for a service to connect.
    /// </summary>
    public class OAuthFlowContinueEventArgs : EventArgs
    {
        /// <summary>
        /// The calendar service provider for the account.
        /// </summary>
        public EmailProvider EmailProvider { get; set; }

        /// <summary>
        /// The OAuth code acquired. Null if no code was acquired.
        /// </summary>
        public string Code { get; private set; }

        public OAuthFlowContinueEventArgs(EmailProvider emailProvider, string code)
        {
            this.EmailProvider = emailProvider;
            this.Code = code;
        }
    }
}
