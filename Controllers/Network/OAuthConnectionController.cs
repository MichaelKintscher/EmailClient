using Application.Config;
using Application.Network;
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers.Network
{
    public class OAuthConnectionController
    {
        private IOAuthService OAuthService { get; set; }
        private IStorageProvider StorageProvider { get; set; }

        /// <summary>
        /// The file name of the file stonring the Google Calendar API OAuth token.
        /// </summary>
        private string TokenFileName
        {
            get => this.OAuthService.Name + "_token.json";
        }

        /// <summary>
        /// Default constructor - takes in a OAuth Service Provider and Storage Provider as dependency injection.
        /// </summary>
        /// <param name="oAuthService">The OAuth service provider the connection is associated with.</param>
        /// <param name="storageProvider">The storage provider used for persisting connection data.</param>
        public OAuthConnectionController(IOAuthService oAuthService, IStorageProvider storageProvider)
        {
            this.OAuthService = oAuthService;
            this.StorageProvider = storageProvider;
        }

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
