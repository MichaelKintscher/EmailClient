﻿using Application.Messages;
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

        #region Event Handlers
        /// <summary>
        /// Handles when an email is moved on the view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_EmailMoved(object sender, EventArguments.EmailsMovedEventArgs e)
        {
            // Move the emails to the new message box.
            MessageBoxManager manager = new MessageBoxManager(WindowsStorageProvider.Instance);
            manager.MoveEmailsToMessageBoxAsync(e.Emails, e.Destination.ID);
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
            view.EmailMoved += View_EmailMoved;

            // Get the emails for the view.
            EmailAccountManager emailManager = new EmailAccountManager();
            List<Email> emails = await emailManager.GetEmailsAsync();

            // Convert the list of emails to a dictionary keyed by assigned Message Box.
            Dictionary<string, List<Email>> emailsDict = emails.GroupBy(e => e.MessageBoxID)
                                                               .ToDictionary(g => g.Key, g => g.ToList());

            // Assign the emails not in other boxes to the inbox.
            List<Email> inboxEmails = emailsDict[Guid.Empty.ToString()];
            foreach (Email email in inboxEmails)
            {
                view.AddEmailToInbox(email);
            }

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
