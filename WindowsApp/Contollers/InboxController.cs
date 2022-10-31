using Application.Messages;
using Application.Messages.Emails;
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
            // Subscribe to the page's events.

            // Get the emails for the view.
            EmailAccountManager emailManager = new EmailAccountManager();
            List<Email> emails = await emailManager.GetEmailsAsync();
            foreach (Email email in emails)
            {
                view.AddEmailToInbox(email);
            }

            // Get the message boxes for the view.
            MessageBoxManager boxesManager = new MessageBoxManager(WindowsStorageProvider.Instance);
            List<MessageBox> boxes = await boxesManager.GetMessageBoxesAsync();
            foreach (MessageBox box in boxes)
            {
                // Get a list of accounts associated with the messages in these message boxes.
                List<ServiceProviderAccount> accounts = AppConfigManager.GetServiceProviderAccounts();
                foreach (ServiceProviderAccount account in accounts)
                {
                    // Get a list of message providers associated with the account.
                    List<IMessageService> messageServices = AppConfigManager.GetMessageServicesForAccount(account.ID);
                    foreach (IMessageService messageService in messageServices)
                    {
                        MessagesManager messagesManager = new MessagesManager(account, messageService, WindowsStorageProvider.Instance);
                        box.Messages = await messagesManager.GetMessagesAsync();
                    }
                }

                view.AddMessageBox(box);
            }

            // Store a reference to the page.
            this.View = view;
        }
        #endregion
    }
}
