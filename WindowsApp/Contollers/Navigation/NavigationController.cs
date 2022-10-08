using Domain.Common;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsApp.Contollers.Navigation.EventArguments;
using WindowsApp.Pages;

namespace WindowsApp.Contollers.Navigation
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

        #region Constructors
        /// <summary>
        /// The default constructor - initializes the properties to null.
        /// </summary>
        public NavigationController()
        {
            this.MainWindow = null;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles when the main window navigated to a new page. This event must be
        /// handled to access the instance of the page actually displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Navigated(object sender, NavigatedEventArgs e)
        {
            this.InitializePageControllerAsync(e.PageNavigatedTo);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the navigation controller to manage navigation based on the given main window.
        /// </summary>
        /// <param name="mainWindow">The main window for managing app navigation on.</param>
        public void Initialize(MainWindow mainWindow)
        {
            // Subscribe to the root page's events.
            mainWindow.Navigated += MainWindow_Navigated;

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
            this.MainWindow.Navigate(PageTypes.Home);
        }

        /// <summary>
        /// Initializes the cotroller for the given page.
        /// </summary>
        /// <param name="page"></param>
        /// <exception cref="NotImplementedException">Thrown if the given page type is not supported by the Navigation Controller.</exception>
        /// <returns></returns>
        public async Task InitializePageControllerAsync(Page page)
        {
            if (page is HomePage homePage)
            {
                // Initialize a controller for the home page.
                await HomeController.Instance.InitializeAsync(homePage);
            }
            if (page is SettingsPage settingsPage)
            {
                // Initialize a controller for the settings page.
                await SettingsController.Instance.InitializeAsync(settingsPage);
            }
            else
            {
                // The page is not of a type supported by this controller class.
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
