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
        private SettingsPage View { get; set; }

        private OAuthConnectionManager ConnectionManager { get; set; }
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
        #endregion

        #region Methods
        /// <summary>
        /// Initializes 
        /// </summary>
        /// <param name="view"></param>
        internal void Initialize(SettingsPage view)
        {
            // Subscribe to the page's events.
            view.ChangeAccountConnectionRequested += this.View_ChangeAccountConnectionRequested;

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
            this.ConnectionManager = new OAuthConnectionManager(serviceProvider, WindowsStorageProvider.Instance);

            // Get the OAuth uri, and display it on the settings page.
            string accountName = this.ConnectionManager.GetServiceProviderName();
            Uri oauthUri = this.ConnectionManager.GetOAuthStartUri();
            await this.View.ShowServiceOAuthCodeUIAsync(accountName, oauthUri);
        }
        #endregion
    }
}
