using Application.Config;
using Domain.Common;
using Domain.Messages;
using Domain.Messages.Emails;
using InterfaceAdapters.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Windows.Storage;
using WindowsOS.Persistence.Exceptions;

namespace WindowsOS.Persistence
{
    /// <summary>
    /// Implements the storage provider interface for Windows.
    /// </summary>
    public class WindowsStorageProvider : Singleton<WindowsStorageProvider>, IStorageProvider
    {
        #region Properties - OAuth
        /// <summary>
        /// The file path the API credentials are stored in.
        /// </summary>
        private static readonly string credentialsFilePath = "/Assets/Config/credentials.json";
        #endregion

        #region Methods - Messages
        /// <summary>
        /// Saves a list of messages.
        /// </summary>
        /// <param name="messagesFileName">The name to give the messages file.</param>
        /// <param name="emails">The list of messages to save.</param>
        /// <returns></returns>
        public async Task SaveMessagesAsync(string messagesFileName, List<Email> emails)
        {
            // Serialize the data to the file format.
            string messagesString = EmailAdapter.Serialize(emails);

            // Create the file, replacing the old if it already exists, and write the data to the file.
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(messagesFileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, messagesString);
        }

        /// <summary>
        /// Gets a list of the messages.
        /// </summary>
        /// <param name="messagesFileName">The name of the messages file.</param>
        /// <returns></returns>
        public async Task<List<Email>> LoadMessagesAsync(string messagesFileName)
        {
            // Initialize the list.
            List<Email> messages = new List<Email>();

            // Try to read the list from the file.
            IStorageItem storageItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync(messagesFileName);
            if (storageItem is StorageFile file)
            {
                // Read the data from the file.
                string fileContent = await FileIO.ReadTextAsync(file);
                messages = EmailAdapter.Deserialize(fileContent);
            }

            return messages;
        }
        #endregion

        #region Methods - Message Boxes
        /// <summary>
        /// Saves a list of the message boxes.
        /// </summary>
        /// <param name="messageBoxesFileName">The name to give the message boxes file.</param>
        /// <param name="messageBoxes">The list of message boxes to save.</param>
        /// <returns></returns>
        public async Task SaveMessageBoxesAsync(string messageBoxesFileName, List<MessageBox> messageBoxes)
        {
            // Serialize the data to the file format.
            string messageBoxesString = MessageBoxAdapter.Serialize(messageBoxes);

            // Create the file, replacing the old if it already exists, and write the data to the file.
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(messageBoxesFileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, messageBoxesString);
        }

        /// <summary>
        /// Gets the list of message boxes.
        /// </summary>
        /// <param name="messageBoxesFileName">The name of the message boxes file.</param>
        /// <returns></returns>
        public async Task<List<MessageBox>> LoadMessageBoxesAsync(string messageBoxesFileName)
        {
            // Initialize the list.
            List<MessageBox> messageBoxes = new List<MessageBox>();

            // Try to read the list from the file.
            IStorageItem storageItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync(messageBoxesFileName);
            if (storageItem is StorageFile file)
            {
                // Read the data from the file.
                string fileContent = await FileIO.ReadTextAsync(file);
                messageBoxes = MessageBoxAdapter.Deserialize(fileContent);
            }

            return messageBoxes;
        }
        #endregion

        #region Methods - OAuth
        /// <summary>
        /// Saves a list of the service provider accounts connected to the app.
        /// </summary>
        /// <param name="accountsFileName">The name to give the connected service provider accounts file.</param>
        /// <param name="accounts">The list of connected service provider accounts to save.</param>
        /// <returns></returns>
        public async Task SaveConnectedAccountsAsync(string accountsFileName, List<ServiceProviderAccount> accounts)
        {
            // Serialize the data to the file format.
            string accountsString = ServiceProviderAccountAdapter.Serialize(accounts);

            // Create the file, replacing the old if it already exists, and write the data to the file.
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(accountsFileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, accountsString);
        }

        /// <summary>
        /// Gets a list of the service provider accounts connected to the app.
        /// </summary>
        /// <param name="accountsFileName">The name to give the connected service provider accounts file.</param>
        /// <returns></returns>
        public async Task<List<ServiceProviderAccount>> LoadConnectedAccountsAsync(string accountsFileName)
        {
            // Initialize the list.
            List<ServiceProviderAccount> accounts = new List<ServiceProviderAccount>();

            // Try to read the list from the file.
            IStorageItem storageItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync(accountsFileName);
            if (storageItem is StorageFile file)
            {
                // Read the data from the file.
                string fileContent = await FileIO.ReadTextAsync(file);
                accounts = ServiceProviderAccountAdapter.Deserialize(fileContent);
            }

            return accounts;
        }

        /// <summary>
        /// Saves the authorization data for all acounts with the current API
        /// connection to a token file with the given name.
        /// </summary>
        /// <param name="tokenFileName">The name to give the token save file.</param>
        public async Task SaveConnectionDataAsync(string tokenFileName, Dictionary<string, OAuthToken> tokens)
        {
            // Save the token data, if there is any.
            if (tokens != null)
            {
                // Get the token data string.
                string tokenString = OAuthTokenAdapter.SerializeTokenData(tokens);

                // Create the token data file and write the token data string to it.
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile tokenFile = await folder.CreateFileAsync(tokenFileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(tokenFile, tokenString);
            }
        }

        /// <summary>
        /// Trys to load the OAuth token data from the token file.
        /// </summary>
        /// <returns>Whether the token data was successfully loaded.</returns>
        public async Task<Dictionary<string, OAuthToken>> TryLoadTokenDataAsync(string tokenFileName)
        {
            Dictionary<string, OAuthToken> tokenDataCollection = new Dictionary<string, OAuthToken>();

            System.Diagnostics.Debug.WriteLine("Trying to load API token file...");
            // Read the text from the file.
            string lines = "";
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile tokenFile = await folder.GetFileAsync(tokenFileName);
                lines = await FileIO.ReadTextAsync(tokenFile);
            }
            catch (Exception ex)
            {
                // An IO exception occured, so return false.
                System.Diagnostics.Debug.WriteLine("Error accessing: " + ApplicationData.Current.LocalFolder.Path);
                return null;
            }

            // Return false if the read data is empty or whitespace.
            if (String.IsNullOrWhiteSpace(lines))
            {
                System.Diagnostics.Debug.WriteLine("API token file was empty!");
                return null;
            }

            // Try to convert the token response to token data.
            try
            {
                // NOTE: An exception is thrown if the JSON contained in the string
                //      is ill-formed.
                tokenDataCollection = OAuthTokenAdapter.DeserializeTokenData(lines);
            }
            catch (Exception ex)
            {
                return null;
            }

            // The data was successfully loaded if this point is reached.
            return tokenDataCollection;
        }

        /// <summary>
        /// Loads the client app's credentials for the Google API from the file they are stored in.
        /// </summary>
        /// <returns>The client app's credentials for the API.</returns>
        public ApiCredential LoadApiCredentials()
        {
            string fullPath = Windows.ApplicationModel.Package.Current.InstalledPath + WindowsStorageProvider.credentialsFilePath;
            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string credentialsString = reader.ReadToEnd();

                    ApiCredential credentials;
                    try
                    {
                        credentials = ApiCredentialAdapter.Deserialize(credentialsString);
                    }
                    catch (Exception ex)
                    {
                        throw new WindowsStorageException("Error occured while attempting to deserialize loaded API credentials.", ex);
                    }

                    return credentials;
                }
            }
        }
        #endregion
    }
}
