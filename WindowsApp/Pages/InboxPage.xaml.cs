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
        /// The list of message boxes to display.
        /// </summary>
        private ObservableCollection<MessageBox> MessageBoxes { get; set; }

        /// <summary>
        /// The source of the list of emails currently being dragged. Is null
        /// if there source is not from this page.
        /// </summary>
        private MessageBoxControl MessagesDragSource { get; set; }
        #endregion

        #region Constructors
        public InboxPage()
        {
            this.InitializeComponent();

            this.MessageBoxes = new ObservableCollection<MessageBox>();
            this.MessagesDragSource = null;
        }
        #endregion

        #region Event Handlers
        private void MessageBoxesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is MessageBox box)
            {
                System.Diagnostics.Debug.WriteLine(box.Name + " has been clicked!");
            }
        }

        /// <summary>
        /// Handles when emails are dragged from a message box on the page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageBoxControl_MessageDragStarting(object sender, Controls.EventArguments.DragMessagesStartingEventArgs e)
        {
            if (sender is MessageBoxControl messageBoxControl)
            {
                System.Diagnostics.Debug.WriteLine("Drag of " + e.Emails.Count + " emails started from " + messageBoxControl.MessageBoxName);
                // Store a reference to the message box control acting as the source of the drag.
                this.MessagesDragSource = messageBoxControl;
            }
        }

        /// <summary>
        /// Handles when emails are dropped on a message box on the page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageBoxControl_MessageDropCompleted(object sender, Controls.EventArguments.MessageDropCompletedEventArgs e)
        {
            if (sender is MessageBoxControl messageBoxControl)
            {
                // If the source of the emails being dragged was a message box on this page AND
                //      NOT the same as the destination message box...
                if (this.MessagesDragSource != null)
                {
                    // Remove the emails that were dropped from the source message box.
                    foreach (Email email in e.Emails)
                    {
                        this.MessagesDragSource.RemoveEmail(email);
                    }

                    // Printing output for testing in identifying the different cases.
                    if (this.MessagesDragSource != messageBoxControl)
                    {
                        // The drag and drop was between two different message box controls.
                        System.Diagnostics.Debug.WriteLine("Drag of " + e.Emails.Count + " emails completed from " + this.MessagesDragSource.MessageBoxName + " to " + messageBoxControl.MessageBoxName);
                    }
                    else
                    {
                        // The drag and drop source and destination were the same - so it was a reordering.
                        System.Diagnostics.Debug.WriteLine(e.Emails.Count + " emails reordered on " + messageBoxControl.MessageBoxName);
                    }
                }
                else
                {
                    // The drag source was off of this page.
                    System.Diagnostics.Debug.WriteLine("Drag of " + e.Emails.Count + " emails started from off this page.");
                }

                // Clear the messages drag source reference.
                this.MessagesDragSource = null;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add an email to be displayed in the inbox.
        /// </summary>
        /// <param name="email">The email to add.</param>
        public void AddEmailToInbox(Email email)
        {
            this.InboxMessageBoxControl.AddEmail(email);
        }

        /// <summary>
        /// Adds a message box to be displayed.
        /// </summary>
        /// <param name="box">The message box to add.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void AddMessageBox(MessageBox box)
        {
            this.MessageBoxes.Add(box);
        }
        #endregion
    }
}
