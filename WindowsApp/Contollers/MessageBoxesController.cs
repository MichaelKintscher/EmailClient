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

        #region Methods
        /// <summary>
        /// Initializes the controller with the given view.
        /// </summary>
        /// <param name="view">The view this controller will control.</param>
        /// <returns></returns>
        internal async Task InitializeAsync(MessageBoxesPage view)
        {
            // Subscribe to the page's events.

            // Get the message boxes for the view.
            //MessageBoxManager boxesManager = new MessageBoxManager(WindowsStorageProvider.Instance);
            //List<MessageBox> boxes = await boxesManager.GetMessageBoxesAsync();
            //foreach (MessageBox box in boxes)
            //{
            //    view.AddMessageBox(box);
            //}

            // Store a reference to the page.
            this.View = view;
        }
        #endregion
    }
}
