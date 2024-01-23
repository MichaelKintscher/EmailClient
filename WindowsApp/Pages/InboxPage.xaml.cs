using Application.Messages;
using Domain.Common;
using Domain.Messages;
using Domain.Messages.Emails;
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WindowsApp.Controls;
using WindowsApp.EventArguments;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowsApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InboxPage : Page
    {
        #region Properties
        /// <summary>
        /// A list of connected service provider accounts.
        /// </summary>
        private ObservableCollection<ServiceProviderAccount> Accounts { get; set; }

        /// <summary>
        /// A list of folders for the current account.
        /// </summary>
        private ObservableCollection<string> Folders { get; set; }

        /// <summary>
        /// A list of messages in the current folder.
        /// </summary>
        private ObservableCollection<Email> Messages { get; set; }
        #endregion

        #region Events
        internal delegate void SelectedAccountChangedHandler(object sender, ServiceProviderAccountEventArgs e);
        /// <summary>
        /// Raised when the selected account changes.
        /// </summary>
        internal event SelectedAccountChangedHandler SelectedAccountChanged;
        private void RaiseSelectedAccountChanged(ServiceProviderAccount account)
        {
            // Create the args and call the listening event handlers, if there are any.
            ServiceProviderAccountEventArgs args = new ServiceProviderAccountEventArgs(account);
            this.SelectedAccountChanged?.Invoke(this, args);
        }
        #endregion

        #region Constructors
        public InboxPage()
        {
            this.InitializeComponent();

            // Initialize the collection.
            this.Accounts = new ObservableCollection<ServiceProviderAccount>();
            this.Folders = new ObservableCollection<string>()
            {
                "Inbox",
                "Archive",
                "Drafts",
                "Sent",
                "Trash"
            };
            this.Messages = new ObservableCollection<Email>();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles when the selected account changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccountsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the new selected account and raise the associated event.
            ServiceProviderAccount account = e.AddedItems[0] as ServiceProviderAccount;
            this.RaiseSelectedAccountChanged(account);
        }

        /// <summary>
        /// Handles when an item in the messages list view is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessagesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Email email)
            {
                this.DisplayMessage(email);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Selects the given account.
        /// </summary>
        /// <param name="account">The account to select.</param>
        public void SelectAccount(ServiceProviderAccount account)
        {
            this.AccountsListView.SelectedItem = account;
        }

        /// <summary>
        /// Add an already connected account to display.
        /// </summary>
        /// <param name="account">The account to add.</param>
        public void AddConnectedAccout(ServiceProviderAccount account)
        {
            // Store a reference to the page.
            this.Accounts.Add(account);
        }

        /// <summary>
        /// Add a message to the message list.
        /// </summary>
        /// <param name="message">The message to add.</param>
        public void AddMessageToList(Email message)
        {
            this.Messages.Add(message);
        }

        /// <summary>
        /// Clears the message list.
        /// </summary>
        public void ClearMessageList()
        {
            this.Messages.Clear();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Displays the given email in the webview.
        /// </summary>
        /// <param name="email">The email to display.</param>
        private void DisplayMessage(Email email)
        {
            this.ContentWebView.NavigateToString(email.Body);
        }
        #endregion
    }
}
