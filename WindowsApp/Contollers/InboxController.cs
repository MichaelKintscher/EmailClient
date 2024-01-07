using Application.Messages;
using Application.Messages.Emails;
using Application.Network;
using Domain.Common;
using Domain.Messages;
using Domain.Messages.Emails;
using Network.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsApp.Contollers.Common;
using WindowsApp.Pages;
using WindowsOS.Persistence;

namespace WindowsApp.Contollers
{
    /// <summary>
    /// The controller for managing the inbox page.
    /// </summary>
    internal class InboxController : PageController<InboxPage, InboxController>
    {
        #region Constructors
        /// <summary>
        /// Default constructor - initializes all properties to null.
        /// </summary>
        public InboxController()
            : base()
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the controller with the given view.
        /// </summary>
        /// <param name="view">The view this controller will control.</param>
        /// <returns></returns>
        internal async Task InitializeAsync(InboxPage view)
        {
            // Add the accounts logged into with this app to the page.
            OAuthConnectionManager connectionManager = new OAuthConnectionManager(GmailAPI.Instance, WindowsStorageProvider.Instance);
            await connectionManager.LoadConnectionsAsync();
            List<ServiceProviderAccount> accounts = await connectionManager.GetConnectionsAsync();
            foreach (ServiceProviderAccount account in accounts)
            {
                view.AddConnectedAccout(account);
            }

            this.View = view;
        }
        #endregion
    }
}
