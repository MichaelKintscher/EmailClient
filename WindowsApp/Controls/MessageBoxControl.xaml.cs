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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowsApp.Controls
{
    public sealed partial class MessageBoxControl : UserControl, INotifyPropertyChanged
    {
        #region Properties
        /// <summary>
        /// A list of emails in the inbox.
        /// </summary>
        private ObservableCollection<Email> Messages { get; set; }

        private string messageBoxName;
        /// <summary>
        /// The name of the message box.
        /// </summary>
        public string MessageBoxName
        {
            get => this.messageBoxName;
            set
            {
                this.messageBoxName = value;
                this.RaisePropertyChanged("MessageBoxName");
            }
        }

        public double MessageBoxMinHeight
        {
            get => this.MessageBoxListView.MinHeight;
            set
            {
                this.MessageBoxListView.MinHeight = value;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised when a bindable property of the control has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        public MessageBoxControl()
        {
            this.InitializeComponent();

            // Initialize the collection.
            this.MessageBoxName = string.Empty;
            this.Messages = new ObservableCollection<Email>();
        }

        #region Methods
        /// <summary>
        /// Add an email to be displayed in the message box.
        /// </summary>
        /// <param name="email">The email to add.</param>
        public void AddEmail(Email email)
        {
            this.Messages.Add(email);
        }
        #endregion
    }
}
