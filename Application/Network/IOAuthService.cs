using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Network
{
    public interface IOAuthService
    {
        /// <summary>
        /// The name of the API.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the URI to start the OAuth 2.0 authoization flow.
        /// </summary>
        /// <returns>The URI to start the OAuth 2.0 authoization flow</returns>
        public Uri GetOAuthUri();

        /// <summary>
        /// Initializes the token data cache and refreshes each token if needed.
        /// </summary>
        /// <param name="tokens">A ditionary of OAuthToken values keyed by account ID.</param>
        public Task InitializeTokenDataAsync(Dictionary<string, OAuthToken> tokens);

        /// <summary>
        /// Gets the cached token data if there is any.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, OAuthToken> GetCachedTokenData();

        /// <summary>
        /// Removes the account by revoking the API access token and deleting any locally cached token data.
        /// </summary>
        /// <param name="accountId">The ID for the account assigned by the app.</param>
        /// <returns></returns>
        public Task<bool> RemoveConnectionAsync(string accountId);
    }
}
