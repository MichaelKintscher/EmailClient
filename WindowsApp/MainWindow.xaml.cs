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
using WindowsApp.Contollers.Navigation;
using WindowsApp.Contollers.Navigation.EventArguments;
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
        #region Properties
        /// <summary>
        /// List of values to convert between page types and tags.
        /// </summary>
        private readonly List<(PageTypes Tag, Type Page)> _pages = new List<(PageTypes Tag, Type Page)>
        {
            //(PageTypes.Home, typeof(HomePage)),
            (PageTypes.Settings, typeof(SettingsPage))
        };

        /// <summary>
        /// The page in the NavView prior to executing a requested navigation.
        /// </summary>
        private Page CurrentNavViewPage { get; set; }
        #endregion

        #region Events
        internal delegate void NavigationRequestedHandler(object sender, NavigationRequestedEventArgs e);
        /// <summary>
        /// Raised when the user indicated they want to navigate.
        /// </summary>
        internal event NavigationRequestedHandler NavigationRequested;
        private void RaiseNavigationRequested(PageTypes toPage, bool isBackRequest = false)
        {
            // Create the args and call the listening event handlers, if there are any.
            NavigationRequestedEventArgs args = new NavigationRequestedEventArgs(toPage, isBackRequest);
            this.NavigationRequested?.Invoke(this, args);
        }

        internal delegate void NavigatedHandler(object sender, NavigatedEventArgs e);
        /// <summary>
        /// Raised when a page navigation has completed.
        /// </summary>
        internal event NavigatedHandler Navigated;
        private void RaiseNavigated(Page fromPage, Page toPage)
        {
            // Create the args and call the listening event handlers, if there are any.
            NavigatedEventArgs args = new NavigatedEventArgs(fromPage, toPage);
            this.Navigated?.Invoke(this, args);
        }
        #endregion

        #region Constructors
        public MainWindow()
        {
            this.InitializeComponent();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles relevant setup when the navigation view has loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("NavView Loaded!");

            // Default to the first menu item (home) when loaded.
            this.NavigationView.SelectedItem = NavigationView.MenuItems[0];

            // Add keyboard accelerators for backwards navigation.
            var goBack = new KeyboardAccelerator { Key = Windows.System.VirtualKey.GoBack };
            goBack.Invoked += this.Keyboard_BackInvoked;
            this.NavigationView.KeyboardAccelerators.Add(goBack);

            // ALT routes here
            var altLeft = new KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.Left,
                Modifiers = Windows.System.VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += this.Keyboard_BackInvoked;
            this.NavigationView.KeyboardAccelerators.Add(altLeft);
        }

        /// <summary>
        /// Handles whenever a menu item in the navigation view is invoked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("NavView Item Invoked!");
            if (args.IsSettingsInvoked == true)
            {
                // Settings does not have a tag, as it is handled separately from the other menu items.
                this.RaiseNavigationRequested(PageTypes.Settings);
            }
            else
            {
                // Get the page enum value associated with the selected nav menu item's tag.
                PageTypes toPage = Enum.Parse<PageTypes>(args.InvokedItemContainer.Tag.ToString());
                this.RaiseNavigationRequested(toPage);
            }
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
            this.NavigationView.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                this.NavigationView.SelectedItem = (NavigationViewItem)this.NavigationView.SettingsItem;
                //this.NavigationView.Header = "Settings";
            }
            else if (ContentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                this.NavigationView.SelectedItem = this.NavigationView.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(n => n.Tag.Equals(item.Tag.ToString()));

                //this.NavigationView.Header = ((NavigationViewItem)this.NavigationView.SelectedItem)?.Content?.ToString();
            }

            // Raise the navigated event.
            Page navigatedToPage = e.Content as Page;
            this.RaiseNavigated(this.CurrentNavViewPage, navigatedToPage);
        }

        /// <summary>
        /// Handles when the content frame fails to navigate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ContentFrame Navigation Failed!");
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Handles when a back request is invoked via keyboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Keyboard_BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            this.OnBackRequested();
            args.Handled = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Navigates the child page to the requested page.
        /// </summary>
        /// <param name="page">The page to navigate to</param>
        internal void Navigate(PageTypes page)
        {
            // Get the page from the list with a tag matching the given page type.
            var item = _pages.FirstOrDefault(p => p.Tag.Equals(page));

            // In theory this shouldn't happen... but just in case...
            if (item.Page == null)
            {
                throw new Exception("unexpected");
            }

            // Grab the current page prior to navigation.
            this.CurrentNavViewPage = this.ContentFrame.Content as Page;

            // Tell the content frame inside of the navigation view to navigate pages.
            this.ContentFrame.Navigate(item.Page);
        }

        /// <summary>
        /// Navigates to the previous child page.
        /// </summary>
        public void NavigateBack()
        {
            if (this.ContentFrame.CanGoBack)
            {
                // Grab the current page prior to navigation.
                this.CurrentNavViewPage = this.ContentFrame.Content as Page;

                this.ContentFrame.GoBack();
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Raises a navigation request if the UI is in a valid state to do so.
        /// </summary>
        /// <returns></returns>
        private bool OnBackRequested()
        {
            if (!this.ContentFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed.
            if (this.NavigationView.IsPaneOpen &&
                (this.NavigationView.DisplayMode == NavigationViewDisplayMode.Compact ||
                 this.NavigationView.DisplayMode == NavigationViewDisplayMode.Minimal))
                return false;

            string pageTypeString = this.ContentFrame.BackStack.First().SourcePageType.Name.ToString().Replace("Page", String.Empty);
            PageTypes backToPage = Enum.Parse<PageTypes>(pageTypeString);
            this.RaiseNavigationRequested(backToPage, true);
            return true;
        }
        #endregion
    }
}
