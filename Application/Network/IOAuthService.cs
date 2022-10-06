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
        /// Returns whether the user has authorized the app with the API on the given account.
        /// </summary>
        /// <param name="accountId">The account ID to check the authorization on.</param>
        /// <returns></returns>
        public bool IsAuthorized(string accountId);

        /// <summary>
        /// Gets the URI to start the OAuth 2.0 authoization flow.
        /// </summary>
        /// <returns>The URI to start the OAuth 2.0 authoization flow</returns>
        public Uri GetOAuthUri();

        /// <summary>
        /// Completes the OAuth flow by exchanging the given authorization code for a token.
        /// </summary>
        /// <param name="accountId">The account ID to check the access token of.</param>
        /// <param name="authorizationCode">The authorization code to exchange for the token.</param>
        /// <exception cref="InvalidOperationException">Thrown if this method is called before the API credentials are initialized. Use Initialize() to set the credentials.</exception>
        /// <exception cref="Exception">The given accountId has no data associated with it.</exception>
        public Task GetOauthTokenAsync(string accountId, string authorizationCode);

        /// <summary>
        /// Initializes the token data cache and refreshes each token if needed.
        /// </summary>
        /// <param name="tokens">A ditionary of OAuthToken values keyed by account ID.</param>
        public Task InitializeTokenDataAsync(Dictionary<string, OAuthToken> tokens);

        public abstract Task<ServiceProviderAccount> GetAccountAsync(string accountId);

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
