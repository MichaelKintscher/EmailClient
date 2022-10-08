using Domain.Common;
using Network.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsOS.Persistence;

namespace WindowsApp.Contollers
{
    /// <summary>
    /// The main controller for the app - handles app life cycle and initialization.
    /// </summary>
    internal class AppController : Singleton<AppController>
    {
        #region Properties
        #endregion

        #region Methods
        /// <summary>
        /// Intended to be called upon app launch. Initializes app navigation.
        /// </summary>
        /// <param name="mainWindow">The main window for this initialization of the app.</param>
        public async Task StartAppAsync(MainWindow mainWindow)
        {
            // Initialize the necessary resource interfaces.
            this.InitializeInterfaces();

            // Navigate to the app's starting page.
            NavigationController.Instance.Initialize(mainWindow);
            await NavigationController.Instance.NavigateToStartingPageAsync();
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
