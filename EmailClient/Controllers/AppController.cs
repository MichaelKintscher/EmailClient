using EmailClient.EventArguments;
using EmailClient.Models.AppConfigModels;
using EmailClient.Models.Gmail;
using EmailClient.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailClient.Controllers
{
    /// <summary>
    /// The main controller for the app - handles navigation between app-level processes.
    /// </summary>
    internal class AppController : SingletonController<AppController>
    {
        #region Properties
        /// <summary>
        /// State variable to store a unique identifier for a currently pending
        /// account authorization.
        /// </summary>
        private Guid accountIdPendingAuthorization = Guid.Empty;
        #endregion

        #region Event Handlers - Settings Page
        /// <summary>
        /// Handles when the user requests to change a connection to a service.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsPage_ChangeAccountConnectionRequested(object sender, ChangeAccountConnectionRequestedEventArgs e)
        {
            if (sender is SettingsPage settingsPage)
            {
                switch (e.Action)
                {
                    // A request was issued to connect to the service.
                    case ConnectionAction.Connect:
                        this.StartOAuthAsync();
                        settingsPage.ShowServiceOAuthCodeUIAsync(e.AccoutId);
                        break;

                    // A request was issued to retry connecting to the service.
                    case ConnectionAction.RetryConnect:
                        this.StartOAuthAsync();
                        settingsPage.ShowServiceOAuthCodeUIAsync(e.AccoutId);
                        break;

                    // A request was issued to disconnect the service.
                    case ConnectionAction.Disconnect:
                        this.DisconnectAccountAsync(e.AccoutId, settingsPage);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Handles when the user has entered an OAuth code to connect a service.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsPage_OauthCodeAcquired(object sender, OAuthFlowContinueEventArgs e)
        {
            // Validate the code.
            if (String.IsNullOrWhiteSpace(e.Code))
            {
                // The code is definitely invalid; no point in reaching out to the server.
                if (sender is SettingsPage settingsPage)
                {
                    string errorMessage = "No code was entered!";
                    settingsPage.ShowOAuthErrorUIAsync(errorMessage);
                }
            }
            else
            {
                // Complete the OAuth flow and clear the state data for the pending authorization.
                GmailAPI.Instance.GetOauthTokenAsync(this.accountIdPendingAuthorization.ToString(), e.Code);
                this.accountIdPendingAuthorization = Guid.Empty;
            }
        }

        /// <summary>
        /// Handles when the user cancells the pending connect account request from the settings page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsPage_ConnectionRequestCancelled(object sender, OAuthFlowContinueEventArgs e)
        {
            // Clear the associated state value.
            this.accountIdPendingAuthorization = Guid.Empty;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Wrapps the call to the Google Calendar API so that await can be used.
        /// </summary>
        /// <returns></returns>
        private async Task StartOAuthAsync()
        {
            this.accountIdPendingAuthorization = await GmailAPI.Instance.StartOAuthAsync();
        }

        /// <summary>
        /// Disconnects an account.
        /// </summary>
        /// <param name="accountId">The ID of the account to remove.</param>
        /// <param name="settingsPage">The settings page instance to update with the disconnected account.</param>
        /// <returns></returns>
        private async Task DisconnectAccountAsync(string accountId, SettingsPage settingsPage)
        {
            // Remove the account's connection with Google.
            await GmailAPI.Instance.RemoveAccount(accountId);

            // Remove cahced account data.
            await AppConfig.Instance.RemoveAccountAsync(accountId);

            // Remove any necessary data from the UI associated with the removed account.

            // Remove the account from the settings page.
            EmailProviderAccount accountToRemove = settingsPage.Accounts.Where(a => a.ID == accountId).FirstOrDefault();
            if (accountToRemove != null)
            {
                settingsPage.Accounts.Remove(accountToRemove);
            }
        }
        #endregion
    }
}
