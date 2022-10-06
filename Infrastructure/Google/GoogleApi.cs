using Domain.Common;
using Network.Common;
using Network.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Network.Google
{
    public abstract class GoogleApi<T> : OAuthApi<T> where T : new()
    {
        #region Constants
        /// <summary>
        /// The base uri for Google OAuth 2.0
        /// </summary>
        public static readonly string oauthBaseUri = "https://accounts.google.com/o/oauth2/v2/auth";
        /// <summary>
        /// The redirect uri for the OAuth 2.0 flow. This is the special value Google uses to indicate a manual redirect.
        /// See: https://developers.google.com/identity/protocols/oauth2/native-app
        /// </summary>
        public static readonly string oauthRedirectUri = "urn:ietf:wg:oauth:2.0:oob";
        /// <summary>
        /// The endpoint uri for exchanging the authorization code for an access token.
        /// </summary>
        private static readonly string oauthTokenEndpoint = "https://oauth2.googleapis.com/token";
        /// <summary>
        /// The endpoint uri for revoking an authorized token.
        /// </summary>
        private static readonly string oauthRevokeTokenEndpoint = "https://oauth2.googleapis.com/revoke";
        #endregion

        #region Properties
        /// <summary>
        /// The scopes within the API the app is accessing. See: https://developers.google.com/gmail/api/auth/scopes
        /// </summary>
        private static string[] scopes { get; set; }
        /// <summary>
        /// The name of the application to present to the API.
        /// </summary>
        private static string applicationName { get; set; }
        #endregion

        #region Constructors
        public GoogleApi(string apiName, string applicationName, string[] scopes)
            : base(GoogleApi<T>.oauthBaseUri, GoogleApi<T>.oauthTokenEndpoint, GoogleApi<T>.oauthRedirectUri,GoogleApi<T>.oauthRevokeTokenEndpoint,
                        GoogleApi<T>.GetOAuthQueryString, GoogleApi<T>.GetTokenExchangeParams,
                        GoogleApi<T>.ConvertResponseToToken, GoogleApi<T>.GetTokenRefreshParams)
        {
            // Initialize the API constants.
            GoogleApi<T>.applicationName = applicationName;
            GoogleApi<T>.scopes = scopes;

            // Initalize API properties.
            this.Name = apiName;
        }
        #endregion

        #region Methods

        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the query string for a request to the Google OAuth endpoint.
        /// </summary>
        /// <param name="credentials">The client app's credentials for the Google API.</param>
        /// <returns>The query string for a request to the Google OAuth endpoint. DOES NOT include the base url for the endpoint.</returns>
        private static string GetOAuthQueryString(ApiCredential credentials)
        {
            // Build the scopes string as a space deliminated list, per the requirement
            //      specified here: https://developers.google.com/identity/protocols/oauth2/native-app#uwp
            StringBuilder scopesString = new StringBuilder();
            foreach (string scope in GoogleApi<T>.scopes)
            {
                scopesString.Append(scope);
                scopesString.Append(" ");
            }

            // Create the start URI
            string startUrl = "?client_id=" + credentials.ClientId +
                              "&redirect_uri=" + GoogleApi<T>.oauthRedirectUri +
                              "&response_type=code" +
                              "&scope=" + scopesString.ToString();
            return startUrl;
        }

        /// <summary>
        /// Gets the parameters for Google's OAuth 2.0 token exchange point.
        /// </summary>
        /// <param name="authorizationCode">The authorization code returned by the OAuth endpoint.</param>
        /// <param name="redirectUri">The redirect URI for the OAuth flow.</param>
        /// <param name="credentials">The client app's credentials for the Google API.</param>
        /// <returns>An IList of key value pairs representing the HTTP POST parameters.</returns>
        private static IList<KeyValuePair<string, string>> GetTokenExchangeParams(string authorizationCode, string redirectUri, ApiCredential credentials)
        {
            // Parameter names are documented at: https://developers.google.com/identity/protocols/oauth2/native-app#step-2:-send-a-request-to-googles-oauth-2.0-server
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", credentials.ClientId),
                new KeyValuePair<string, string>("client_secret", credentials.ClientSecret),
                new KeyValuePair<string, string>("code", authorizationCode),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            };
        }

        /// <summary>
        /// Converts the token response from the Google OAuth API to its corresponding OAuthToken object.
        /// </summary>
        /// <param name="responseContent">The string of the Json object to convert into a token response object.</param>
        /// <returns>The token response object containing the data in the given content string.</returns>
        protected static OAuthToken ConvertResponseToToken(string responseContent)
        {
            // Parse the response json content.
            // Response format is documented at: https://developers.google.com/identity/protocols/oauth2/native-app#handlingresponse
            JsonNode? responseJson = JsonNode.Parse(responseContent);
            if (responseJson == null)
            {
                throw new ResponseFormatException("Response from API call to get token was not in the expected format.");
            }

            string token = responseJson["access_token"] != null ? responseJson["access_token"]!.GetValue<string>() : "";
            long expiresInSeconds = responseJson["expires_in"] != null ? responseJson["expires_in"]!.GetValue<long>() : 0;
            string tokenType = responseJson["token_type"] != null ? responseJson["token_type"]!.GetValue<string>() : "";
            string scope = responseJson["scope"] != null ? responseJson["scope"]!.GetValue<string>() : "";
            string refreshToken = responseJson["refresh_token"] != null ? responseJson["refresh_token"]!.GetValue<string>() : "";

            // If the response content contains an issued time, use that. Otherwise, default to Utc now.
            //      This value does not exist from the actual response JSON from Google's API, so this
            //      is a new API response if the value does not exist, and thus the issued time should be
            //      set to now. Otherwise, the respose JSON is a saved copy, and the saved value should
            //      be used.
            DateTime issuedTime = responseJson["issued_time"] != null ? DateTime.Parse(responseJson["issued_time"]!.GetValue<string>()) : DateTime.UtcNow;

            // Create and return a new instance of the OAuthToken class.
            return new OAuthToken()
            {
                AccessToken = token,
                TokenType = tokenType,
                ExpiresInSeconds = expiresInSeconds,
                RefreshToken = refreshToken,
                Scope = scope,
                IdToken = "",
                IssuedUtc = issuedTime
            };
        }

        /// <summary>
        /// Gets the parameters for Google's OAuth 2.0 token refresh.
        /// </summary>
        /// <param name="refreshToken">The refresh token to be exchanged at the API endpoint for a new authorization token.</param>
        /// <param name="credentials">The client app's credentials for the Google API.</param>
        /// <returns>An IList of key value pairs representing the HTTP POST parameters.</returns>
        private static IList<KeyValuePair<string, string>> GetTokenRefreshParams(string refreshToken, ApiCredential credentials)
        {
            // Parameter names are documented at: https://developers.google.com/identity/protocols/oauth2/native-app#exchange-authorization-code
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", credentials.ClientId),
                new KeyValuePair<string, string>("client_secret", credentials.ClientSecret),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token")
            };
        }

        /// <summary>
        /// Converts the token response object into a string containing a serialized Json representation of that object.
        /// </summary>
        /// <param name="tokenData">The response to serialize into a string.</param>
        /// <returns>The string containing a serialized Json representation of the given Token Response object.</returns>
        private static string ConvertTokenToJsonString(OAuthToken tokenData)
        {
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("access_token", JsonValue.Create<string>(tokenData.AccessToken));

            // Set the expiration time (lifespan of the token) to zero seconds if no value exists.
            double expirationTime = tokenData.ExpiresInSeconds.HasValue ? (double)tokenData.ExpiresInSeconds.Value : 0.0;
            jsonObject.Add("expires_in", JsonValue.Create<double>(expirationTime));

            jsonObject.Add("token_type", JsonValue.Create<string>(tokenData.TokenType));
            jsonObject.Add("scope", JsonValue.Create<string>(tokenData.Scope));
            jsonObject.Add("refresh_token", JsonValue.Create<string>(tokenData.RefreshToken));
            jsonObject.Add("issued_time", JsonValue.Create<string>(tokenData.IssuedUtc.ToString()));

            return jsonObject.ToJsonString();
        }
        #endregion
    }
}
