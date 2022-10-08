using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WindowsApp.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowsApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        #region Event Handlers
        /// <summary>
        /// Handles relevant setup when the navigation view has loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("NavView Loaded!");
        }

        /// <summary>
        /// Handles whenever a menu item in the navigation view is invoked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("NavView Item Invoked!");
        }

        /// <summary>
        /// Handles whenever back navigation is requested from the navigation view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("NavView Back Requested!");
        }

        /// <summary>
        /// Handles logic once the content frame has completed a successful navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ContentFrame Navigated!");
        }

        /// <summary>
        /// Handles when the content frame fails to navigate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ContentFrame Navigation Failed!");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Displays the requested page.
        /// </summary>
        /// <param name="page">The page to display.</param>
        internal void Navigate(Page page)
        {
            this.ContentFrame.Content = page;
        }
        #endregion
    }
}
