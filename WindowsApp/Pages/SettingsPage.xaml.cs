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
using System.Threading.Tasks;
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

        internal delegate void OauthCodeAcquiredHandler(object sender, OAuthFlowContinueEventArgs e);
        /// <summary>
        /// Raised when the a request is issued to connect a service.
        /// </summary>
        internal event OauthCodeAcquiredHandler OauthCodeAcquired;
        private void RaiseOauthCodeAcquired(EmailProvider emailProvider, string code)
        {
            // Create the args and call the listening event handlers, if there are any.
            OAuthFlowContinueEventArgs args = new OAuthFlowContinueEventArgs(emailProvider, code);
            this.OauthCodeAcquired?.Invoke(this, args);
        }

        internal delegate void ConnectionRequestCancelledHandler(object sender, OAuthFlowContinueEventArgs e);
        /// <summary>
        /// Raised when the current request issued to connect a service is cancelled.
        /// </summary>
        internal event ConnectionRequestCancelledHandler ConnectionRequestCancelled;
        private void RaiseConnectionRequestCancelled(EmailProvider emailProvider)
        {
            // Create the args and call the listening event handlers, if there are any.
            OAuthFlowContinueEventArgs args = new OAuthFlowContinueEventArgs(emailProvider, null);
            this.ConnectionRequestCancelled?.Invoke(this, args);
        }
        #endregion

        #region Constructors
        public SettingsPage()
        {
            this.InitializeComponent();

            // Initialize the collection.
            this.Accounts = new ObservableCollection<ServiceProviderAccount>();
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
            if (sender is Button button)
            {
                // Get the acount ID and then raise the RaiseChangeAccountConnectionRequested event.
                string accountId = button.Tag.ToString();
                this.RaiseChangeAccountConnectionRequested(accountId, EmailProvider.Google, ConnectionAction.Disconnect);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add an already connected account to display.
        /// </summary>
        /// <param name="account">The account to add.</param>
        public void AddConnectedAccout(ServiceProviderAccount account)
        {
            this.Accounts.Add(account);
        }

        /// <summary>
        /// Remove a connected account from the display.
        /// </summary>
        /// <param name="accountId">The ID of the account to remove.</param>
        public void RemoveConnectedAccount(string accountId)
        {
            ServiceProviderAccount account = this.Accounts.Where(a => a.ID == accountId).FirstOrDefault();
            this.Accounts.Remove(account);
        }

        /// <summary>
        /// Shows the service OAuth Code dialog.
        /// </summary>
        /// <param name="serviceName">The name of the service to show the dialog box for.</param>
        /// <returns></returns>
        public async Task ShowServiceOAuthCodeUIAsync(string serviceName, Uri oauthUri)
        {
            // Set the webview to the oauth Uri.
            this.OAuthWebView.Source = oauthUri;

            // Show the OAuth code dialog box, and get a response from the user.
            var result = await this.FinishAddingServiceDialog.ShowAsync();

            switch (result)
            {
                case ContentDialogResult.None:
                    // Close the dialog and raise the connection request cancelled event.
                    this.RaiseConnectionRequestCancelled(EmailProvider.Google);
                    break;
                case ContentDialogResult.Primary:
                    // Raise the code acquired event.
                    this.RaiseOauthCodeAcquired(EmailProvider.Google, this.ServiceOauthCodeTextBox.Text);
                    break;
                case ContentDialogResult.Secondary:
                    // Close the dialog and raise the connection request cancelled event.
                    this.RaiseConnectionRequestCancelled(EmailProvider.Google);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Shows the error UI.
        /// </summary>
        /// <param name="errorMessage">The error message to display.</param>
        /// <returns></returns>
        public async Task ShowOAuthErrorUIAsync(string errorMessage)
        {
            // Set the error message.
            this.OAuthErrorTextBlock.Text = errorMessage;

            var result = await this.OauthErrorDialog.ShowAsync();

            switch (result)
            {
                case ContentDialogResult.None:
                    // Nothing to do... just close the dialog.
                    break;
                case ContentDialogResult.Primary:
                    // Raise the RaiseChangeAccountConnectionRequested event to an retry connecting.
                    this.RaiseChangeAccountConnectionRequested(null, EmailProvider.Google, ConnectionAction.RetryConnect);
                    break;
                case ContentDialogResult.Secondary:
                    // Nothing to do... just close the dialog.
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Shows the dialog to confirm whether to remove the given connected account.
        /// </summary>
        /// <param name="account">The connected account to confirm removal of.</param>
        /// <returns>Whether the user confirmed removing the given account.</returns>
        public async Task<bool> ShowConfirmRemoveAccountUIAsync(ServiceProviderAccount account)
        {
            // Display the account data in the dialog's content control, then show the dialog.
            this.AccountToRemoveContentControl.Content = account;
            var result = await this.ConfirmRemoveAccountDialog.ShowAsync();

            bool confirmed = false;
            switch (result)
            {
                case ContentDialogResult.None:
                    // Nothing to do... just clear and close the dialog.
                    this.AccountToRemoveContentControl.Content = null;
                    break;
                case ContentDialogResult.Primary:
                    confirmed = true;
                    break;
                case ContentDialogResult.Secondary:
                    // Nothing to do... just clear and close the dialog.
                    this.AccountToRemoveContentControl.Content = null;
                    break;
                default:
                    break;
            }

            return confirmed;
        }
        #endregion
    }
}
