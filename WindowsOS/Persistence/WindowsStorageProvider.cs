using Application.Config;
using Domain.Common;
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
    public class WindowsStorageProvider : IStorageProvider
    {
        /// <summary>
        /// The file path the API credentials are stored in.
        /// </summary>
        private static readonly string credentialsFilePath = "/Assets/Config/credentials.json";

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
    }
}
