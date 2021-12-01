using EmailClient.EventArguments;
using EmailClient.Models.AppConfigModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EmailClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        #region Properties
        private bool internetConnectionAvailable;
        /// <summary>
        /// Whether an internet connection is currently available.
        /// </summary>
        public bool InternetConnectionAvailable
        {
            get => this.internetConnectionAvailable;
            set
            {
                this.internetConnectionAvailable = value;
                this.RaisePropertyChanged("InternetConnectionAvailable");
            }
        }

        /// <summary>
        /// A list of connected email service provider accounts.
        /// </summary>
        public ObservableCollection<EmailProviderAccount> Accounts { get; set; }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raise the PropertChanged event for the given property name.
        /// </summary>
        /// <param name="name">Name of the property changed.</param>
        public void RaisePropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public delegate void ChangeAccountConnectionRequestedHandler(object sender, ChangeAccountConnectionRequestedEventArgs e);
        /// <summary>
        /// Raised when the a request is issued to change the connection to a service.
        /// </summary>
        public event ChangeAccountConnectionRequestedHandler ChangeAccountConnectionRequested;
        private void RaiseChangeAccountConnectionRequested(string accountId, EmailProvider emailProvider, ConnectionAction action)
        {
            // Create the args and call the listening event handlers, if there are any.
            ChangeAccountConnectionRequestedEventArgs args = new ChangeAccountConnectionRequestedEventArgs(accountId, emailProvider, action);
            this.ChangeAccountConnectionRequested?.Invoke(this, args);
        }

        public delegate void OauthCodeAcquiredHandler(object sender, OAuthFlowContinueEventArgs e);
        /// <summary>
        /// Raised when the a request is issued to connect a service.
        /// </summary>
        public event OauthCodeAcquiredHandler OauthCodeAcquired;
        private void RaiseOauthCodeAcquired(EmailProvider emailProvider, string code)
        {
            // Create the args and call the listening event handlers, if there are any.
            OAuthFlowContinueEventArgs args = new OAuthFlowContinueEventArgs(emailProvider, code);
            this.OauthCodeAcquired?.Invoke(this, args);
        }

        public delegate void ConnectionRequestCancelledHandler(object sender, OAuthFlowContinueEventArgs e);
        /// <summary>
        /// Raised when the current request issued to connect a service is cancelled.
        /// </summary>
        public event ConnectionRequestCancelledHandler ConnectionRequestCancelled;
        private void RaiseConnectionRequestCancelled(EmailProvider emailProvider)
        {
            // Create the args and call the listening event handlers, if there are any.
            OAuthFlowContinueEventArgs args = new OAuthFlowContinueEventArgs(emailProvider, null);
            this.ConnectionRequestCancelled?.Invoke(this, args);
        }
        #endregion

        public SettingsPage()
        {
            this.InitializeComponent();

            // Initialize the collections.
            this.Accounts = new ObservableCollection<EmailProviderAccount>();
            this.InternetConnectionAvailable = true;
        }

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
        /// Shows the service OAuth Code dialog.
        /// </summary>
        /// <param name="serviceName">The name of the service to show the dialog box for.</param>
        /// <returns></returns>
        public async Task ShowServiceOAuthCodeUIAsync(string serviceName)
        {
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
        #endregion
    }
}
