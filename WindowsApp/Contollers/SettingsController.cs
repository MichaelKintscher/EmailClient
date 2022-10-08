﻿using Application.Network;
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
            // Use the action argument to determine which process to start.
            switch (e.Action)
            {
                case ConnectionAction.Connect:
                    // Start a new Oauth flow.
                    this.StartOAuthFlowAsync(GmailAPI.Instance);
                    break;
                case ConnectionAction.RetryConnect:
                    // Start a new Oauth flow.
                    this.StartOAuthFlowAsync(GmailAPI.Instance);
                    break;
                case ConnectionAction.Disconnect:
                    // Start the account disconnect process.
                    this.DisconnectAccountAsync(e.AccoutId, GmailAPI.Instance);
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

        /// <summary>
        /// Handles when a pending OAuth flow completes (account is authorized).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OAuthAccount_Authorized(object sender, Application.Network.EventArguments.ApiAuthorizedEventArgs e)
        {
            // Ensure that the authorization was a success.
            if (e.AuthorizationSuccess == false)
            {
                // The authorization failed.
                string errorMessage = "Authorizing account with " + e.ApiName + " failed!";
                this.ShowErrorUi(errorMessage);
            }
            else
            {
                // The authorization was a success, so finish adding the new account.
                this.FinishAddingAccountAsync(e.AccountID);
            }
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
            await connectionManager.LoadConnectionsAsync();
            List<ServiceProviderAccount> accounts = await connectionManager.GetConnectionsAsync();
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
        /// <param name="code">The authorization code provided by the API after the user authenticated and authorized.</param>
        /// <exception cref="InvalidOperationException">Thrown if there is no pending connection to continue the OAuth flow for.</exception>
        /// <returns></returns>
        private async Task TryContinueOAuthFlowAsync(string code)
        {
            // Ensure there is a pending connection to complete.
            if (this.PendingConnectionManager == null)
            {
                throw new InvalidOperationException("No pending connection to complete. Use StartOAuthFlowAsync() to begin a new connection before calling TryContinueOAuthFlowAsync().");
            }

            // Validate the code.
            if (String.IsNullOrWhiteSpace(code))
            {
                // The code is definitely invalid; no point in reaching out to the server.
                string errorMessage = "No code was entered!";
                await this.ShowErrorUi(errorMessage);
            }
            else
            {
                // Register for the authorized callback and complete the OAuth flow (authorization token exchange).
                GmailAPI.Instance.Authorized += OAuthAccount_Authorized;
                Guid accountId = Guid.NewGuid();
                await GmailAPI.Instance.GetOauthTokenAsync(accountId.ToString(), code);
            }
        }

        /// <summary>
        /// Finishes adding an authorized account.
        /// </summary>
        /// <param name="accountId">The account ID of the account to finish adding.</param>
        /// <returns></returns>
        private async Task FinishAddingAccountAsync(string accountId)
        {
            // Complete the OAuth flow, unregister the callback, and clear the state data for the pending authorization.
            ServiceProviderAccount account = await this.PendingConnectionManager.AddConnectionAsync(accountId);
            this.PendingConnectionManager = null;
            GmailAPI.Instance.Authorized -= this.OAuthAccount_Authorized;

            // Add the new account to the list of accounts to display.
            this.View.AddConnectedAccout(account);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountId">The account ID of the account to remove.</param>
        /// <param name="serviceProvider">The service provider the account is associated with.</param>
        /// <returns></returns>
        private async Task DisconnectAccountAsync(string accountId, IOAuthService serviceProvider)
        {
            // Create the manager.
            OAuthConnectionManager manager = new OAuthConnectionManager(serviceProvider, WindowsStorageProvider.Instance);

            // Prompt the user to confirm removal of the account.
            ServiceProviderAccount account = await manager.GetConnectionAsync(accountId);
            bool removalConfirmed = await this.View.ShowConfirmRemoveAccountUIAsync(account);

            // Only proceed if the user confirmed removing the account.
            if (removalConfirmed)
            {
                // Remove the connection.
                await manager.RemoveConnectionAsync(accountId);

                // Update the displayed list of accounts.
                this.View.RemoveConnectedAccount(accountId);
            }
        }

        /// <summary>
        /// Displays the given error message on the Settings Page UI.
        /// Wraps the error message display in an async method so it can
        /// be called from event handlers.
        /// </summary>
        /// <param name="errorMessage">The error message to display.</param>
        /// <returns></returns>
        private async Task ShowErrorUi(string errorMessage)
        {
            await this.View.ShowOAuthErrorUIAsync(errorMessage);
        }
        #endregion
    }
}
