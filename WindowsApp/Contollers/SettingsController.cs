using Controllers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsApp.EventArguments;
using WindowsApp.Pages;

namespace WindowsApp.Contollers
{
    /// <summary>
    /// The controller for managing the app settings.
    /// </summary>
    internal class SettingsController : Singleton<SettingsController>
    {
        #region Properties
        private SettingsPage View { get; set; }
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
                    System.Diagnostics.Debug.WriteLine("Connection Requested!!!");
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
        public void Initialize(SettingsPage view)
        {
            // Subscribe to the page's events.
            view.ChangeAccountConnectionRequested += this.View_ChangeAccountConnectionRequested;

            // Store a reference to the page.
            this.View = view;
        }
        #endregion
    }
}
