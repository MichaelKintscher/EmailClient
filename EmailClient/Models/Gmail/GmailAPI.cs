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
        private static readonly string credentialsFilePath = "/Assets/Config/credentials.json";
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

        /// <summary>
        /// Gets the messages from the given email provider account. This is implemented
        /// based on the documentation at: https://developers.google.com/gmail/api/guides/sync
        /// </summary>
        /// <param name="accountId">The ID for the account assigned by the app.</param>
        /// <returns></returns>
        public async Task<List<Message>> GetMessagesAsync(string accountId)
        {
            // The endpoint for getting gmail messages.
            //      Default parameter returns 100 message IDs.
            string uri = "https://www.googleapis.com/gmail/v1/users/me/messages";

            // Get and parse the content.
            string content = await this.GetAsync(accountId, uri);
            List<string> messageIds = this.ParseMessageIdList(content);

            // TODO: Implement step 2 of "Full Synchronization" as detailed here: https://developers.google.com/gmail/api/guides/sync
            //      This code naively loops through and grabs each message in a separate API call.
            List<Message> messages = new List<Message>();
            foreach (string messageId in messageIds)
            {
                Message message = await this.GetMessageAsync(accountId, messageId);
                messages.Add(message);
            }

            return messages;
        }

        /// <summary>
        /// Gets a single message.
        /// </summary>
        /// <param name="accountId">The ID for the account assigned by the app.</param>
        /// <param name="messageId">The ID for the message assigned by the Gmail API.</param>
        /// <returns></returns>
        public async Task<Message> GetMessageAsync(string accountId, string messageId)
        {
            // The endpoint for getting a gmail message.
            string uri = "https://gmail.googleapis.com/gmail/v1/users/me/messages/";

            // Add the message ID parameter.
            uri += messageId;

            // Get and parse the content.
            string content = await this.GetAsync(accountId, uri);
            Message message = this.ParseMessage(content);

            return message;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Parses a list of message IDs from a message list response. The format
        /// for the content input is documented at: https://developers.google.com/gmail/api/reference/rest/v1/users.messages#Message
        /// </summary>
        /// <param name="content">The message list response. Format is documented at: https://developers.google.com/gmail/api/reference/rest/v1/users.messages#Message</param>
        /// <returns></returns>
        private List<string> ParseMessageIdList(string content)
        {
            // Get the array of message info.
            JsonObject jsonObject = JsonObject.Parse(content);
            JsonArray messagesArray = jsonObject["messages"].GetArray();

            // Create an empty list of messages and parse and add each message.
            List<string> messageIds = new List<string>();
            foreach (var messageJson in messagesArray)
            {
                // Get the object from the JSON array item.
                JsonObject messageInfo = messageJson.GetObject();

                // Parse the message ID and add it to the list.
                string id = messageInfo["id"].GetString();
                messageIds.Add(id);
            }

            return messageIds;
        }

        /// <summary>
        /// Pareses a single message from a message.get response. The format for the
        /// content input is documented at: https://developers.google.com/gmail/api/reference/rest/v1/users.messages/get
        /// </summary>
        /// <param name="content">The message.get response. Format is documented at: https://developers.google.com/gmail/api/reference/rest/v1/users.messages/get</param>
        /// <returns></returns>
        public Message ParseMessage(string content)
        {
            JsonObject jsonObject = JsonObject.Parse(content);

            Message message = new Message();

            // Parse the message ID.
            message.ApiGivenId = jsonObject["id"].GetString();

            // Parse the message snippet as the body.
            //      THIS IS NOT THE FULL MESSAGE BODY.
            message.Body = jsonObject["snippet"].GetString();

            // Parse the message's subject.
            JsonObject payload = jsonObject["payload"].GetObject();
            JsonArray headersArray = payload["headers"].GetArray();
            foreach (var headerJson in headersArray)
            {
                // Get the object from the JSON array item.
                JsonObject headerJsonObject = headerJson.GetObject();
                string headerName = headerJsonObject["name"].GetString();
                if (headerName == "Subject")
                {
                    message.Subject = headerJsonObject["value"].GetString();
                }
            }

            return message;
        }
        #endregion
    }
}
