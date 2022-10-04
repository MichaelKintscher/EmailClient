using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    /// <summary>
    /// Represents a client app's API credentials.
    /// </summary>
    public class ApiCredential
    {
        /// <summary>
        /// The client app's ID for an API.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The client app's secret for an API.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Default constructor - creates an instance with empty string ID and Secret.
        /// </summary>
        public ApiCredential()
        {
            this.ClientId = string.Empty;
            this.ClientSecret = string.Empty;
        }
    }
}
