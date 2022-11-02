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

            // Convert the list of emails to a dictionary keyed by assigned Message Box.
            Dictionary<string, List<Email>> emailsDict = emails.GroupBy(e => e.MessageBoxID)
                                                               .ToDictionary(g => g.Key, g => g.ToList());

            // Get the message boxes for the view.
            MessageBoxManager boxesManager = new MessageBoxManager(WindowsStorageProvider.Instance);
            List<MessageBox> boxes = await boxesManager.GetMessageBoxesAsync();

            // For each message box...
            foreach (MessageBox box in boxes)
            {
                // If there are any emails in the box, assign them.
                if (emailsDict.ContainsKey(box.ID))
                {
                    box.Messages.AddRange(emailsDict[box.ID]);
                }

                // Add the box to the view.
                view.AddMessageBox(box);
            }

            // Store a reference to the page.
            this.View = view;
        }
        #endregion
    }
}
