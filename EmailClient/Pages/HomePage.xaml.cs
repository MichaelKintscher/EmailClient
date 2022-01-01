using EmailClient.Models;
using EmailClient.Models.Testing;
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

namespace EmailClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public ObservableCollection<Message> Messages;

        public Message TestMessage;

        #region Constructors
        public HomePage()
        {
            this.InitializeComponent();

            // Initialize the properties.
            this.Messages = new ObservableCollection<Message>();

            // Populate the test message.
            this.PopulateTestMessages();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the list of displayed messages. Existing messages are cleared and
        /// replaced with the given messages.
        /// </summary>
        /// <param name="messages">The list of messages to add to the displayed list of messages.</param>
        /// <exception cref="ArgumentNullException">Thrown when the "messages" parameter is null.</exception>
        public void SetMessageStream(List<Message> messages)
        {
            // Throw an exception if the list of messages is null.
            if (messages == null)
            {
                throw new ArgumentNullException(nameof(messages));
            }

            // Remove any existing displayed messages.
            this.Messages.Clear();

            // Add each message in the list to the displayed messages.
            foreach (Message message in messages)
            {
                this.Messages.Add(message);
            }
        }
        #endregion

        #region Helper Methods
        private void PopulateTestMessages()
        {
            this.Messages = new ObservableCollection<Message>(TestDataGenerator.GetTestMessages(10));
            this.TestMessage = this.Messages.First();
        }
        #endregion
    }
}
