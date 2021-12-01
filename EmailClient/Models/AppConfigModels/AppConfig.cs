using EmailClient.Managers;
using EmailClient.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace EmailClient.Models.AppConfigModels
{
    /// <summary>
    /// Represents configuration data about the app.
    /// </summary>
    public class AppConfig : Singleton<AppConfig>
    {
        #region Constants
        private static readonly string ConfigFileName = "EmailConfig.json";
        #endregion

        #region Properties
        /// <summary>
        /// The list of accounts the user has connected with the app.
        /// </summary>
        private List<EmailProviderAccount> Accounts { get; set; }
        #endregion

        #region Constructors
        public AppConfig()
        {
            this.Accounts = null;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Saves all config contents to a config file.
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            // There are no accounts to save.
            if (this.Accounts == null)
            {
                return;
            }

            // For each account in the list of accounts...
            JsonArray accountsArray = new JsonArray();
            foreach (EmailProviderAccount account in this.Accounts)
            {
                // Serialize the account data.
                JsonObject accountJson = EmailProviderAccountManager.Serialize(account);

                // Add any data assoicated with the account.

                // Add the account to array.
                accountsArray.Add(accountJson);
            }

            // Store the array in a json object.
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("accounts", accountsArray);

            // Get a reference to the file, and create it if it does not exist.
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(AppConfig.ConfigFileName, CreationCollisionOption.ReplaceExisting);

            // Save the json object to the file.
            await FileIO.WriteTextAsync(file, jsonObject.Stringify());
        }
        #endregion

        #region Methods - Accounts
        /// <summary>
        /// Gets a list of accounts the user has connected with the app.
        /// </summary>
        /// <returns></returns>
        public async Task<List<EmailProviderAccount>> GetAccountsAsync()
        {
            // Initialize the list if it is uninitialized.
            if (this.Accounts == null)
            {
                this.Accounts = await this.InitializeAccountsAsync();
            }

            return new List<EmailProviderAccount>(this.Accounts);
        }

        /// <summary>
        /// Adds the given account to the list of accounts.
        /// </summary>
        /// <param name="account">The account to add to the list of accounts.</param>
        /// <returns></returns>
        public async Task AddAccountAsync(EmailProviderAccount account)
        {
            // Initialize the list if it is uninitialized.
            if (this.Accounts == null)
            {
                this.Accounts = await this.InitializeAccountsAsync();
            }

            this.Accounts.Add(account);
        }

        /// <summary>
        /// Removes the account with the given ID from the list of accounts and clears any associated data.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task RemoveAccountAsync(string accountId)
        {
            // Initialize the list if it is uninitialized.
            if (this.Accounts == null)
            {
                this.Accounts = await this.InitializeAccountsAsync();
            }

            EmailProviderAccount account = this.Accounts.Where(a => a.ID == accountId).FirstOrDefault();
            if (account != null)
            {
                this.Accounts.Remove(account);
            }

            // Clear any data associated with this account.
        }

        /// <summary>
        /// Gets the provider-assigned ID for a account, given the app-assigned account ID.
        /// </summary>
        /// <param name="accountId">The unique ID of the account given locally by the app.</param>
        /// <returns></returns>
        public async Task<string> GetProviderAccountId(string accountId)
        {
            // Initialize the list if it is uninitialized.
            if (this.Accounts == null)
            {
                this.Accounts = await this.InitializeAccountsAsync();
            }

            EmailProviderAccount account = this.Accounts.Where(a => a.ID == accountId).FirstOrDefault();

            return account != null ? account.ProviderGivenID : "";
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Initializes the list of accounts the user has connected with the app from the stored data.
        /// </summary>
        /// <returns>The list of hidden calendar records, or an empty list if there is a file error.</returns>
        private async Task<List<EmailProviderAccount>> InitializeAccountsAsync()
        {
            // Initialize the list.
            List<EmailProviderAccount> accounts = new List<EmailProviderAccount>();

            // Try to read the list from the file.
            IStorageItem storageItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync(AppConfig.ConfigFileName);
            if (storageItem is StorageFile file)
            {
                // Read the data from the file.
                string fileContent = await FileIO.ReadTextAsync(file);

                // Parse the data from the file.
                JsonObject jsonObject = JsonObject.Parse(fileContent);
                JsonArray accountsArray = jsonObject["accounts"].GetArray();

                // For each account in the array...
                foreach (var accountJsonValue in accountsArray)
                {
                    // This is necessary because of the type iterated over in the JsonArray.
                    JsonObject accountJson = accountJsonValue.GetObject();

                    // Parse the account data.
                    EmailProviderAccount account = EmailProviderAccountManager.Deserialize(accountJson);

                    // Add a new hidden calendar record to the list.
                    accounts.Add(account);
                }
            }

            return accounts;
        }
        #endregion
    }
}
