using Application.Config;
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Network
{
    public class OAuthConnectionManager
    {
        #region Properties
        /// <summary>
        /// A reference to the OAuth service this connection manager manages.
        /// </summary>
        private IOAuthService OAuthService { get; set; }

        /// <summary>
        /// A reference to the storage provider this connection manager will interface with.
        /// </summary>
        private IStorageProvider StorageProvider { get; set; }

        /// <summary>
        /// The file name of the file stonring the Google Calendar API OAuth token.
        /// </summary>
        private string TokenFileName
        {
            get => this.OAuthService.Name + "_token.json";
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor - takes in a OAuth Service Provider and Storage Provider as dependency injection.
        /// </summary>
        /// <param name="oAuthService">The OAuth service provider the connection is associated with.</param>
        /// <param name="storageProvider">The storage provider used for persisting connection data.</param>
        public OAuthConnectionManager(IOAuthService oAuthService, IStorageProvider storageProvider)
        {
            this.OAuthService = oAuthService;
            this.StorageProvider = storageProvider;
        }
        #endregion

        /// <summary>
        /// Removes a connection to the API service this manager manages.
        /// </summary>
        /// <param name="accountId">The account ID of the account to remove.</param>
        /// <returns></returns>
        public async Task RemoveConnection(string accountId)
        {
            // Remove the cached connection data from the API.
            bool removed = await this.OAuthService.RemoveConnectionAsync(accountId);

            // Save the updated connection data if it changed.
            if (removed)
            {
                await this.SaveConnection();
            }
        }

        /// <summary>
        /// Saves the authorization data for the current connection to the OAuth API.
        /// </summary>
        /// <returns></returns>
        public async Task SaveConnection()
        {
            // Get the updated cached token data collection.
            Dictionary<string, OAuthToken> tokenData = this.OAuthService.GetCachedTokenData();
            // Save the updated collection.
            await this.StorageProvider.SaveConnectionDataAsync(this.TokenFileName, tokenData);
        }
    }
}
