using Application.Persistence;
using CleanArchitecture.Windows.Persistence;
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
    public class WindowsStorageProvider : WindowsStorageProviderBase<WindowsStorageProvider>, IStorageProvider
    {
        #region Properties - OAuth
        /// <summary>
        /// The file path the API credentials are stored in.
        /// </summary>
        private static readonly string credentialsFilePath = "/Assets/Config/credentials.json";
        #endregion

        #region Methods - OAuth
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
                await base.SaveAsync(new List<Dictionary<string, OAuthToken>>() { tokens }, tokenFileName, new OAuthTokenAdapter());
            }
        }

        /// <summary>
        /// Trys to load the OAuth token data from the token file.
        /// </summary>
        /// <param name="tokenFileName">The name to give the token save file.</param>
        /// <returns>Whether the token data was successfully loaded.</returns>
        public async Task<Dictionary<string, OAuthToken>> TryLoadTokenDataAsync(string tokenFileName)
        {
            List<Dictionary<string, OAuthToken>> tokens = await base.LoadAsync<Dictionary<string, OAuthToken>>(tokenFileName, new OAuthTokenAdapter());
            return (tokens != null && tokens.Count > 0) ? tokens[0] : new Dictionary<string, OAuthToken>();
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
