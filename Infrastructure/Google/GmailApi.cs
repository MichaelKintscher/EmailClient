using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Application.Common.Commands.CreateServiceProviderAccount;
using Domain.Common;
using Domain.Emails;
using Network.Common.Exceptions;

namespace Network.Google
{
    /// <summary>
    /// Wrapper class for interfacing with the Gmail API. This class encapsulates
    ///  all external dependencies on the Gmail v1 API.
    /// </summary>
    internal class GmailAPI : GoogleApi<GmailAPI>
    {
        #region Constants
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
            : base(GmailAPI.apiName, GmailAPI.applicationName, GmailAPI.scopes)
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the user's Google account.
        /// </summary>
        /// <param name="accountId">The ID for the account assigned by the app.</param>
        /// <returns></returns>
        /// <exception cref="ResponseFormatException">Thrown if expected data from the API call response is missing.</exception>
        public async Task<ServiceProviderAccount> GetAccountAsync(string accountId)
        {
            // The endpoint for getting Google account info.
            string uri = "https://www.googleapis.com/oauth2/v3/userinfo";

            // Get and parse the content.
            string content = await this.GetAsync(accountId, uri);
            JsonNode? accountJson = JsonNode.Parse(content);
            if (accountJson == null)
            {
                throw new ResponseFormatException("No account data was successfully parsed from the API call to get the user account.");
            }

            string providerId;
            string userName;
            try
            {
                providerId = accountJson["sub"].GetValue<string>();
                userName = accountJson["email"].GetValue<string>();
            }
            catch (NullReferenceException ex)
            {
                throw new ResponseFormatException("The user account data returned by the API call is missing an expected value.", ex);
            }

            // Put an empty string for the picture URI if none exists.
            string pictureUri = accountJson["picture"] == null ? string.Empty : accountJson["picture"]!.GetValue<string>();

            // Create the account object.
            ServiceProviderAccount account = ServiceProviderAccountFactory.CreateServiceProviderAccount(
                accountId,
                "Google",
                providerId,
                userName,
                pictureUri,
                true
                );

            return account;
        }

        /// <summary>
        /// Gets the messages from the given email provider account. This is implemented
        /// based on the documentation at: https://developers.google.com/gmail/api/guides/sync
        /// </summary>
        /// <param name="accountId">The ID for the account assigned by the app.</param>
        /// <returns></returns>
        public async Task<List<Email>> GetMessagesAsync(string accountId)
        {
            // The endpoint for getting gmail messages.
            //      Default parameter returns 100 message IDs.
            string uri = "https://www.googleapis.com/gmail/v1/users/me/messages";

            // Get and parse the content.
            string content = await this.GetAsync(accountId, uri);
            List<string> messageIds = this.ParseMessageIdList(content);

            // TODO: Implement step 2 of "Full Synchronization" as detailed here: https://developers.google.com/gmail/api/guides/sync
            //      This code naively loops through and grabs each message in a separate API call.
            List<Email> messages = new List<Email>();
            foreach (string messageId in messageIds)
            {
                Email message = await this.GetMessageAsync(accountId, messageId);
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
        public async Task<Email> GetMessageAsync(string accountId, string messageId)
        {
            // The endpoint for getting a gmail message.
            string uri = "https://gmail.googleapis.com/gmail/v1/users/me/messages/";

            // Add the message ID parameter.
            uri += messageId;

            // Get and parse the content.
            string content = await this.GetAsync(accountId, uri);
            Email message = this.ParseMessage(content);

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
        /// <exception cref="ResponseFormatException">Thrown if expected data from the API call response is missing.</exception>
        private List<string> ParseMessageIdList(string content)
        {
            // Get the array of message info.
            JsonNode? jsonObject = JsonNode.Parse(content);
            if (jsonObject == null)
            {
                throw new ResponseFormatException("No account data was successfully parsed from the API call to get the list of message IDs on the user's account.");
            }

            JsonArray messagesArray;
            try
            {
                messagesArray = jsonObject["messages"].AsArray();
            }
            catch (NullReferenceException ex)
            {
                throw new ResponseFormatException("The user's message ID list data returned by the API call is missing an expected value.", ex);
            }

            // Create an empty list of messages and parse and add each message.
            List<string> messageIds = new List<string>();
            foreach (var messageJson in messagesArray)
            {
                // Ignore any null messages.
                if (messageJson == null)
                {
                    continue;
                }

                // Get the object from the JSON array item.
                JsonNode messageInfo = messageJson.AsObject();

                // Ignore any messages with missing ID values.
                if (messageInfo["id"] == null)
                {
                    continue;
                }

                // Parse the message ID and add it to the list.
                string id = messageInfo["id"]!.GetValue<string>();
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
        /// <exception cref="ResponseFormatException">Thrown if expected data from the API call response is missing.</exception>
        public Email ParseMessage(string content)
        {
            JsonNode jsonObject = JsonNode.Parse(content);

            Email message = new Email();

            // Parse the message ID.
            try
            {
                message.ProviderGivenID = jsonObject["id"].GetValue<string>();
            }
            catch (NullReferenceException ex)
            {
                throw new ResponseFormatException("The user's message data returned by the API call is missing an expected value.", ex);
            }

            // Parse the message snippet as the body.
            //      THIS IS NOT THE FULL MESSAGE BODY.
            message.Body = jsonObject["snippet"].GetValue<string>();

            // Parse the message's subject.
            JsonArray headersArray;
            try
            {
                JsonObject payload = jsonObject["payload"].AsObject();
                headersArray = payload["headers"].AsArray();
            }
            catch (NullReferenceException ex)
            {
                throw new ResponseFormatException("The user's message data returned by the API call is missing an expected value.", ex);
            }
            
            foreach (var headerJson in headersArray)
            {
                // Skip any null headers.
                if (headerJson == null)
                {
                    continue;
                }

                // Get the object from the JSON array item.
                JsonObject headerJsonObject = headerJson.AsObject();
                string headerName;
                try
                {
                    headerName = headerJsonObject["name"].GetValue<string>();
                }
                catch (NullReferenceException ex)
                {
                    throw new ResponseFormatException("The user's message data returned by the API call is missing an expected value.", ex);
                }
                
                if (headerName == "Subject")
                {
                    message.Subject = headerJsonObject["value"] == null ? string.Empty : headerJsonObject["value"]!.GetValue<string>();
                }
            }

            return message;
        }
        #endregion
    }
}
