using EmailClient.Models.ApiModels;
using EmailClient.Models.AppConfigModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace EmailClient.Models.Gmail
{
    /// <summary>
    /// Wrapper class for interfacing with the Gmail API. This class encapsulates
    ///  all external dependencies on the Gmail v1 API.
    /// </summary>
    internal class GmailAPI : GoogleApi<GmailAPI>
    {
        #region Constants
        /// <summary>
        /// The file path the API credentials are stored in.
        /// </summary>
        private static readonly string credentialsFilePath = "Assets/Config/credentials.json";
        /// <summary>
        /// The scopes within the API the app is accessing. See: https://developers.google.com/gmail/api/auth/scopes
        /// </summary>
        private static readonly string[] scopes = { "https://www.googleapis.com/auth/gmail.modify", "email", "profile" };
        /// <summary>
        /// The name of the application to present to the API.
        /// </summary>
        private static readonly string applicationName = "Email Client";
        /// <summary>
        /// The name of this API (for use in the app to distinguish between APIs).
        /// </summary>
        private static readonly string apiName = "Gmail";
        #endregion

        #region Properties
        
        #endregion

        #region Constructors
        public GmailAPI()
            : base(GmailAPI.apiName, GmailAPI.applicationName, GmailAPI.scopes, GmailAPI.credentialsFilePath)
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the user's Google account.
        /// </summary>
        /// <param name="accountId">The ID for the account assigned by the app.</param>
        /// <returns></returns>
        public async Task<EmailProviderAccount> GetAccountAsync(string accountId)
        {
            // The endpoint for getting Google account info.
            string uri = "https://www.googleapis.com/oauth2/v3/userinfo";

            // Get and parse the content.
            string content = await this.GetAsync(accountId, uri);
            JsonObject accountJson = JsonObject.Parse(content);
            string providerId = accountJson["sub"].GetString();
            string userName = accountJson["email"].GetString();
            string pictureUri = accountJson["picture"].GetString();

            // Create the account object.
            EmailProviderAccount account = new EmailProviderAccount()
            {
                ID = accountId,
                Provider = EmailProvider.Google,
                ProviderGivenID = providerId,
                FriendlyName = "Test Account",
                Username = userName,
                PictureUri = pictureUri,
                PictureLocalUri = "",
                Connected = true,
                LastSynced = DateTime.Now
            };

            return account;
        }

        #endregion

        #region Helper Methods

        #endregion
    }
}
