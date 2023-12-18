using Application.Persistence;
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Network
{
    /// <summary>
    /// Manages OAuth connections.
    /// </summary>
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
        /// The file name of the file storing the OAuth token for the OAuth service this connection manager manages.
        /// </summary>
        private string TokenFileName
        {
            get => this.OAuthService.Name + "_token.json";
        }

        /// <summary>
        /// The file name of the file storing the connected accounts for the OAuth service this connection manager manages.
        /// </summary>
        private string AccountsFileName
        {
            get => this.OAuthService.Name + "_accounts.json";
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

        #region Methods
        /// <summary>
        /// Gets the URI to start the OAuth 2.0 authoization flow.
        /// </summary>
        /// <returns>The URI to start the OAuth 2.0 authoization flow</returns>
        public Uri GetOAuthStartUri()
        {
            return this.OAuthService.GetOAuthUri();
        }

        /// <summary>
        /// Gets the name of the OAuth service provider.
        /// </summary>
        /// <returns></returns>
        public string GetServiceProviderName()
        {
            return this.OAuthService.Name;
        }

        /// <summary>
        /// Gets the account connected to this app with the given account ID.
        /// </summary>
        /// <param name="accountId">The newly assigned account ID of the account to add.</param>
        /// <returns>The account matching the given ID, or null if no account is found.</returns>
        public async Task<ServiceProviderAccount> GetConnectionAsync(string accountId)
        {
            List<ServiceProviderAccount> accounts = await this.GetConnectionsAsync();

            return accounts.Where(a => a.ID == accountId).FirstOrDefault();
        }

        /// <summary>
        /// Gets a list of the accounts connected to this app.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ServiceProviderAccount>> GetConnectionsAsync()
        {
            // Get the list of connections.
            List<ServiceProviderAccount> accounts = new List<ServiceProviderAccount>();
            try
            {
                accounts = await this.StorageProvider.LoadAsync<ServiceProviderAccount>(this.AccountsFileName);
            }
            catch (Exception ex)
            {

            }

            return accounts;
        }

        /// <summary>
        /// Adds a new connection to the API service this manager managess. Throws an error if the connection is not authorized.
        /// </summary>
        /// <param name="accountId">The newly assigned account ID of the account to add.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if the given account ID is not authorized with the API.</exception>
        public async Task<ServiceProviderAccount> AddConnectionAsync(string accountId)
        {
            // Ensure the connection is authorized.
            bool authorized = this.OAuthService.IsAuthorized(accountId);
            if (authorized == false)
            {
                throw new InvalidOperationException("Account is not authorized! Authorize the account with the service provider " +
                                                    this.OAuthService.Name + " before attempting to add a connection.");
            }

            // Get the account data.
            ServiceProviderAccount account = await this.OAuthService.GetAccountAsync(accountId);

            // Add the new account to the list, and then save the new account.
            List<ServiceProviderAccount> accounts = await this.GetConnectionsAsync();
            accounts.Add(account);
            await this.SaveConnectionsAsync(accounts);

            return account;
        }

        /// <summary>
        /// Removes a connection to the API service this manager manages.
        /// </summary>
        /// <param name="accountId">The account ID of the account to remove.</param>
        /// <returns></returns>
        public async Task RemoveConnectionAsync(string accountId)
        {
            // Remove the cached connection data from the API.
            bool removed = await this.OAuthService.RemoveConnectionAsync(accountId);

            // Save the updated connection data if it changed.
            if (removed)
            {
                // Remove the account from the list, and then save the new list.
                List<ServiceProviderAccount> accounts = await this.GetConnectionsAsync();
                ServiceProviderAccount accountToRemove = accounts.Where(a => a.ID == accountId).FirstOrDefault();
                accounts.Remove(accountToRemove);
                await this.SaveConnectionsAsync(accounts);
            }
        }

        /// <summary>
        /// Saves the authorization data for the current connection to the OAuth API.
        /// </summary>
        /// <returns></returns>
        public async Task SaveConnectionsAsync(List<ServiceProviderAccount> accounts)
        {
            // Get the updated cached token data collection.
            Dictionary<string, OAuthToken> tokenData = this.OAuthService.GetCachedTokenData();
            // Save the updated collection.
            await this.StorageProvider.SaveConnectionDataAsync(this.TokenFileName, tokenData);

            // Save the list of connected accounts.
            await this.StorageProvider.SaveAsync<ServiceProviderAccount>(accounts, this.AccountsFileName);
        }

        /// <summary>
        /// Initializes the list of connections in the OAuth Service Provider.
        /// </summary>
        /// <returns></returns>
        public async Task LoadConnectionsAsync()
        {
            // Load and restore the cached token data collection.
            Dictionary<string, OAuthToken> tokenData = await this.StorageProvider.TryLoadTokenDataAsync(this.TokenFileName);
            await this.OAuthService.InitializeTokenDataAsync(tokenData);
        }
        #endregion
    }
}
