using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Persistence
{
    /// <summary>
    /// Extends the core IStorageProvider interface with some application-specific methods.
    /// </summary>
    public interface IStorageProvider : CleanArchitecture.Core.Application.IStorageProvider
    {
        /// <summary>
        /// Saves the authorization data for all acounts with the current API
        /// connection to a token file with the given name.
        /// </summary>
        /// <param name="tokenFileName">The name to give the token save file.</param>
        public Task SaveConnectionDataAsync(string tokenFileName, Dictionary<string, OAuthToken> tokens);

        /// <summary>
        /// Trys to load the OAuth token data from the token file.
        /// </summary>
        /// <param name="tokenFileName">The name to give the token save file.</param>
        /// <returns>Whether the token data was successfully loaded.</returns>
        public Task<Dictionary<string, OAuthToken>> TryLoadTokenDataAsync(string tokenFileName);

        /// <summary>
        /// Loads the client app's credentials for the Google API from the file they are stored in.
        /// </summary>
        /// <returns>The client app's credentials for the API.</returns>
        public ApiCredential LoadApiCredentials();
    }
}
