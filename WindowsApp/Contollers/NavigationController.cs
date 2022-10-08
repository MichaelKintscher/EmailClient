using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsApp.Pages;

namespace WindowsApp.Contollers
{
    /// <summary>
    /// Controls navigation within the app.
    /// </summary>
    internal class NavigationController : Singleton<NavigationController>
    {
        #region Properties
        /// <summary>
        /// The main window for app navigation.
        /// </summary>
        public MainWindow MainWindow { get; private set; }
        #endregion

        /// <summary>
        /// Initializes the navigation controller to manage navigation based on the given main window.
        /// </summary>
        /// <param name="mainWindow">The main window for managing app navigation on.</param>
        public void Initialize(MainWindow mainWindow)
        {
            // Subscribe to the root page's events.

            // Set the given page as the root page.
            this.MainWindow = mainWindow;
        }

        /// <summary>
        /// Navigates to the starting page for the app.
        /// </summary>
        /// <returns></returns>
        public async Task NavigateToStartingPageAsync()
        {
            // Logic to determine which page to go to will go here.

            // Just navigate to the settings page for now.
            await this.NavigateToSettingsPageAsync();
        }

        /// <summary>
        /// Navigates to the Settings Page.
        /// </summary>
        /// <returns></returns>
        public async Task NavigateToSettingsPageAsync()
        {
            // Navigate to the settings page.
            SettingsPage settingsPage = new SettingsPage();
            await SettingsController.Instance.InitializeAsync(settingsPage);
            this.MainWindow.Navigate(settingsPage);
        }
    }
}
