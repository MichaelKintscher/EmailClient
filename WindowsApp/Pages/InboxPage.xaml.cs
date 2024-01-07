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
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add an already connected account to display.
        /// </summary>
        /// <param name="account">The account to add.</param>
        public void AddConnectedAccout(ServiceProviderAccount account)
        {
            // Store a reference to the page.
            this.Accounts.Add(account);
        }
        #endregion
    }
}
