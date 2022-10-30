using Domain.Messages;
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
    public sealed partial class MessageBoxesPage : Page
    {
        #region Properties
        /// <summary>
        /// The list of message boxes to display.
        /// </summary>
        private ObservableCollection<MessageBox> MessageBoxes { get; set; }
        #endregion

        #region Events
        internal delegate void CreateMessageBoxRequestedHandler(object sender, CreateMessageBoxRequestedEventArgs e);
        /// <summary>
        /// Raised when a request is issued to create a new message box.
        /// </summary>
        internal event CreateMessageBoxRequestedHandler CreateMessageBoxRequested;
        private void RaiseCreateMessageBoxRequested(string name)
        {
            // Create the args and call the listening event handlers, if there are any.
            CreateMessageBoxRequestedEventArgs args = new CreateMessageBoxRequestedEventArgs(name);
            this.CreateMessageBoxRequested?.Invoke(this, args);
        }
        #endregion

        #region Constructors
        public MessageBoxesPage()
        {
            this.InitializeComponent();

            this.MessageBoxes = new ObservableCollection<MessageBox>();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles when the add message box button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMessageBoxButton_Click(object sender, RoutedEventArgs e)
        {
            this.ShowCreateMessageBoxDialogAsync();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a message box to be displayed.
        /// </summary>
        /// <param name="box">The message box to add.</param>
        public void AddMessageBox(MessageBox box)
        {
            this.MessageBoxes.Add(box);
        }
        #endregion

        #region Helper Methods
        private async Task ShowCreateMessageBoxDialogAsync()
        {
            // Show the create message box dialog and get a response from the user.
            var result = await this.CreateMessageBoxDialog.ShowAsync();

            switch (result)
            {
                case ContentDialogResult.None:
                    // Nothing else to do.
                    break;
                case ContentDialogResult.Primary:
                    // Raise the request to create a new message box with the given name.
                    string messageBoxName = this.CreateMessageBoxNameTextBox.Text;
                    this.RaiseCreateMessageBoxRequested(messageBoxName);
                    break;
                case ContentDialogResult.Secondary:
                    // Nothing else to do.
                    break;
                default:
                    break;
            }

            // Clear the textbox on the dialog.
            this.CreateMessageBoxNameTextBox.Text = string.Empty;
        }
        #endregion
    }
}
