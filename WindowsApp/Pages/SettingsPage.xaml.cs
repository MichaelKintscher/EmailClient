using Domain.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Network.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WindowsApp.EventArguments;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowsApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        #region Properties
        /// <summary>
        /// A list of connected service provider accounts.
        /// </summary>
        private ObservableCollection<ServiceProviderAccount> Accounts { get; set; }
        #endregion

        #region Events
        internal delegate void ChangeAccountConnectionRequestedHandler(object sender, ChangeAccountConnectionRequestedEventArgs e);
        /// <summary>
        /// Raised when the a request is issued to change the connection to a service.
        /// </summary>
        internal event ChangeAccountConnectionRequestedHandler ChangeAccountConnectionRequested;
        private void RaiseChangeAccountConnectionRequested(string accountId, EmailProvider emailProvider, ConnectionAction action)
        {
            // Create the args and call the listening event handlers, if there are any.
            ChangeAccountConnectionRequestedEventArgs args = new ChangeAccountConnectionRequestedEventArgs(accountId, emailProvider, action);
            this.ChangeAccountConnectionRequested?.Invoke(this, args);
        }
        #endregion

        #region Constructors
        public SettingsPage()
        {
            this.InitializeComponent();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the button click to connect Google's services.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthenticateGoogleButton_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseChangeAccountConnectionRequested(null, EmailProvider.Google, ConnectionAction.Connect);
        }

        /// <summary>
        /// Handles the button click to remove a signed in account.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveAccountButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
