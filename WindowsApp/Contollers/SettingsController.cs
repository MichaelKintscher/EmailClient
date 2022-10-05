using Application.Network;
using Domain.Common;
using Network.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsApp.EventArguments;
using WindowsApp.Pages;
using WindowsOS.Persistence;

namespace WindowsApp.Contollers
{
    /// <summary>
    /// The controller for managing the app settings.
    /// </summary>
    internal class SettingsController : Singleton<SettingsController>
    {
        #region Properties
        /// <summary>
        /// A reference to the view the controller is controlling.
        /// </summary>
        private SettingsPage View { get; set; }

        /// <summary>
        /// A reference to a connection manager for a pending new OAuth connection.
        /// </summary>
        private OAuthConnectionManager PendingConnectionManager { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor - initializes all properties to null.
        /// </summary>
        public SettingsController()
        {
            this.View = null;
            this.PendingConnectionManager = null;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles when the user requests to change a connection to a service.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_ChangeAccountConnectionRequested(object sender, ChangeAccountConnectionRequestedEventArgs e)
        {
            switch (e.Action)
            {
                case ConnectionAction.Connect:
                    this.StartOAuthFlowAsync(GmailAPI.Instance);
                    break;
                case ConnectionAction.RetryConnect:
                    break;
                case ConnectionAction.Disconnect:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles when the user cancells the pending connect account request from the settings page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_ConnectionRequestCancelled(object sender, OAuthFlowContinueEventArgs e)
        {
            // Clear the state data for the pending authorization.
            this.PendingConnectionManager = null;
        }

        /// <summary>
        /// Handles when the user has entered an OAuth code to connect a service.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_OauthCodeAcquired(object sender, OAuthFlowContinueEventArgs e)
        {
            this.TryContinueOAuthFlowAsync(e.Code);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes 
        /// </summary>
        /// <param name="view"></param>
        internal async Task InitializeAsync(SettingsPage view)
        {
            // Subscribe to the page's events.
            view.ChangeAccountConnectionRequested += this.View_ChangeAccountConnectionRequested;
            view.OauthCodeAcquired += this.View_OauthCodeAcquired;
            view.ConnectionRequestCancelled += View_ConnectionRequestCancelled;

            // Add the accounts logged into with this app to the page.
            OAuthConnectionManager connectionManager = new OAuthConnectionManager(GmailAPI.Instance, WindowsStorageProvider.Instance);
            List<ServiceProviderAccount> accounts = await connectionManager.LoadConnectionsAsync();
            foreach (ServiceProviderAccount account in accounts)
            {
                view.AddConnectedAccout(account);
            }

            // Store a reference to the page.
            this.View = view;
        }

        /// <summary>
        /// Starts the OAuth flow on the Settings Page.
        /// </summary>
        /// <returns>The service provider the OAuth flow is associated with.</returns>
        private async Task StartOAuthFlowAsync(IOAuthService serviceProvider)
        {
            // Create a new OAuth Connection Manager.
            this.PendingConnectionManager = new OAuthConnectionManager(serviceProvider, WindowsStorageProvider.Instance);

            // Get the OAuth uri, and display it on the settings page.
            string accountName = this.PendingConnectionManager.GetServiceProviderName();
            Uri oauthUri = this.PendingConnectionManager.GetOAuthStartUri();
            await this.View.ShowServiceOAuthCodeUIAsync(accountName, oauthUri);
        }

        /// <summary>
        /// Attempts to complete a started OAuth flow from the Settings Page.
        /// </summary>
        /// <param name="code"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        private async Task TryContinueOAuthFlowAsync(string code)
        {
            // Ensure there is a pending connection to complete.
            if (this.PendingConnectionManager == null)
            {
                throw new InvalidOperationException("No pending connection to cmoplete. Use StartOAuthFlowAsync() to begin a new connection before calling TryContinueOAuthFlowAsync().");
            }

            // Validate the code.
            if (String.IsNullOrWhiteSpace(code))
            {
                // The code is definitely invalid; no point in reaching out to the server.
                string errorMessage = "No code was entered!";
                await this.View.ShowOAuthErrorUIAsync(errorMessage);
            }
            else
            {
                // Complete the OAuth flow and clear the state data for the pending authorization.
                Guid accountId = new Guid();
                ServiceProviderAccount account = await this.PendingConnectionManager.AddConnectionAsync(accountId.ToString(), code);
                this.PendingConnectionManager = null;

                // Add the new account to the list of accounts to display.
                this.View.AddConnectedAccout(account);
            }
        }
        #endregion
    }
}
