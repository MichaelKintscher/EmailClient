using Application.Messages;
using Domain.Messages;
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
    /// The controller for managing the message boxes page.
    /// </summary>
    internal class MessageBoxesController : PageController<MessageBoxesPage, MessageBoxesController>
    {
        #region Constructors
        /// <summary>
        /// Default constructor - initializes all properties to null.
        /// </summary>
        public MessageBoxesController()
            : base()
        {

        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles when the user requests to create a new message box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_CreateMessageBoxRequested(object sender, EventArguments.CreateMessageBoxRequestedEventArgs e)
        {
            this.CreateNewMessageBoxAsync(e.Name);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the controller with the given view.
        /// </summary>
        /// <param name="view">The view this controller will control.</param>
        /// <returns></returns>
        internal async Task InitializeAsync(MessageBoxesPage view)
        {
            // Subscribe to the page's events.
            view.CreateMessageBoxRequested += View_CreateMessageBoxRequested;

            // Get the message boxes for the view.
            MessageBoxManager boxesManager = new MessageBoxManager(WindowsStorageProvider.Instance);
            List<MessageBox> boxes = await boxesManager.GetMessageBoxesAsync();
            foreach (MessageBox box in boxes)
            {
                view.AddMessageBox(box);
            }

            // Store a reference to the page.
            this.View = view;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Creates a new message box with the given name and adds it to the view.
        /// </summary>
        /// <param name="name">The name to give the new message box.</param>
        /// <returns></returns>
        private async Task CreateNewMessageBoxAsync(string name)
        {
            // Create the new message box.
            MessageBoxManager manager = new MessageBoxManager(WindowsStorageProvider.Instance);
            MessageBox messageBox = await manager.CreateMessageBoxAsync(name);

            // Add the new message box to the view.
            this.View.AddMessageBox(messageBox);
        }
        #endregion
    }
}
