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
        #region Properties
        /// <summary>
        /// The currently selected account, used as context for responding to events.
        /// </summary>
        private ServiceProviderAccount SelectedAccount { get; set; }
        #endregion

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
            // Subscribe to the page's events.
            view.SelectedAccountChanged += View_SelectedAccountChanged;

            // Add the accounts logged into with this app to the page.
            OAuthConnectionManager connectionManager = new OAuthConnectionManager(GmailAPI.Instance, WindowsStorageProvider.Instance);
            await connectionManager.LoadConnectionsAsync();
            List<ServiceProviderAccount> accounts = await connectionManager.GetConnectionsAsync();
            foreach (ServiceProviderAccount account in accounts)
            {
                view.AddConnectedAccout(account);
            }

            this.View = view;

            // Set the default selected account, if there are any accounts to select.
            if (accounts.Count > 0)
            {
                view.SelectAccount(accounts[0]);
            }
        }

        /// <summary>
        /// Handles when the selected account changes on the view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_SelectedAccountChanged(object sender, EventArguments.ServiceProviderAccountEventArgs e)
        {
            // Update the messages on the message list to the newly selected account.
            GetMessagesFromServerAsync(e.Account.ID);
        }

        /// <summary>
        /// Gets the messages from the server and adds them to the view for the gicen account ID.
        /// </summary>
        /// <param name="accountId">The account to add messages to the server for.</param>
        /// <returns></returns>
        internal async Task GetMessagesFromServerAsync(string accountId)
        {
            // Clear the existing message list.
            this.View.ClearMessageList();

            // Get the messages from the server.
            MessagesManager messagesManager = new MessagesManager(GmailAPI.Instance, WindowsStorageProvider.Instance);
            List<Email> messages = await messagesManager.GetInboxMessagesFromServerAsync(accountId);
            foreach (Email message in messages)
            {
                this.View.AddMessageToList(message);
            }
        }
        #endregion
    }
}
