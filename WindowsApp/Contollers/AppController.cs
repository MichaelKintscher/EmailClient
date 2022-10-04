using Domain.Common;
using Network.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsApp.Pages;
using WindowsOS.Persistence;

namespace WindowsApp.Contollers
{
    /// <summary>
    /// The main controller for the app - handles navigation between app-level processes.
    /// </summary>
    internal class AppController : Singleton<AppController>
    {
        #region Properties
        /// <summary>
        /// The root page for app navigation.
        /// </summary>
        public MainWindow RootPage { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Intended to be called upon app launch. Initializes app navigation.
        /// </summary>
        /// <param name="rootPage">The root page for this initialization of the app.</param>
        public void StartApp(MainWindow rootPage)
        {
            // Initialize the necessary resource interfaces.
            this.InitializeInterfaces();

            // Subscribe to the root page's events.

            // Set the given page as the root page.
            this.RootPage = rootPage;

            // Navigate to the settings page.
            SettingsPage settingsPage = new SettingsPage();
            SettingsController.Instance.Initialize(settingsPage);
            this.RootPage.Navigate(settingsPage);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Initializes the various resource interfaces.
        /// </summary>
        private void InitializeInterfaces()
        {
            // Initialize the Gmail API.
            ApiCredential credentials = WindowsStorageProvider.Instance.LoadApiCredentials();
            GmailAPI.Instance.Initialize(credentials);
        }
        #endregion
    }
}
