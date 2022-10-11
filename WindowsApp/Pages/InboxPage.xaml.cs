﻿using Domain.Emails;
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
        /// A list of emails in the inbox.
        /// </summary>
        private ObservableCollection<Email> InboxEmails { get; set; }
        #endregion

        #region Constructors
        public InboxPage()
        {
            this.InitializeComponent();

            // Initialize the collection.
            this.InboxEmails = new ObservableCollection<Email>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add an email to be displayed in the inbox.
        /// </summary>
        /// <param name="email">The email to add.</param>
        public void AddEmailToInbox(Email email)
        {
            this.InboxEmails.Add(email);
        }
        #endregion
    }
}