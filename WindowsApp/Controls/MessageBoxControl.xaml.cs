using Domain.Messages.Emails;
using InterfaceAdapters.Json;
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
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WindowsApp.Controls.EventArguments;

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

        public delegate void MessageDragStartingEventHandler(object sender, DragMessagesStartingEventArgs e);
        /// <summary>
        /// Raised when a message drag interaction starts.
        /// </summary>
        public event MessageDragStartingEventHandler MessageDragStarting;
        private void RaiseMessageDragStarting(List<Email> emails, DragItemsStartingEventArgs dragMessagesStartingEventArgs)
        {
            // Create the args and call the listening event handlers, if there are any.
            DragMessagesStartingEventArgs args = new DragMessagesStartingEventArgs(emails, dragMessagesStartingEventArgs);
            this.MessageDragStarting?.Invoke(this, args);
        }

        public delegate void MessageDropCompletedEventHandler(object sender, MessageDropCompletedEventArgs e);
        /// <summary>
        /// Raised when a message drop interaction is completed.
        /// </summary>
        public event MessageDropCompletedEventHandler MessageDropCompleted;
        private void RaiseMessageDropCompleted(List<Email> emails, int insertionIndex, DragEventArgs dragEventArgs)
        {
            // Create the args and call the listening event handlers, if there are any.
            MessageDropCompletedEventArgs args = new MessageDropCompletedEventArgs(emails, insertionIndex, dragEventArgs);
            this.MessageDropCompleted?.Invoke(this, args);
        }
        #endregion

        #region Constructors
        public MessageBoxControl()
        {
            this.InitializeComponent();

            // Initialize the collection.
            this.MessageBoxName = string.Empty;
            this.Messages = new ObservableCollection<Email>();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles when a message drag operation starts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageBoxListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            // Verify that at least one item is being dragged.
            if (e.Items.Count < 1)
            {
                return;
            }

            // Get the list of emails being moved.
            List<Email> emails = e.Items.Cast<Email>().ToList();

            // Get a string of the email IDs being moved.
            string itemsString = EmailAdapter.Serialize(emails);

            // Set the content of the DataPackage.
            e.Data.SetText(itemsString);

            // Set the requested operation.
            e.Data.RequestedOperation = DataPackageOperation.Move;

            // Get the list of emails being dragged, and raise the Message Drag Starting event.
            this.RaiseMessageDragStarting(emails, e);
        }

        /// <summary>
        /// Handles when items are dragged over.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageBoxListView_DragOver(object sender, DragEventArgs e)
        {
            // Set the accepted operation.
            e.AcceptedOperation = DataPackageOperation.Move;
        }

        /// <summary>
        /// Handles when items are dropped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MessageBoxListView_Drop(object sender, DragEventArgs e)
        {
            // Verify the correct data format is in the drop package.
            if (e.DataView.Contains(StandardDataFormats.Text) == false)
            {
                return;
            }

            // Get a reference to the defferal.
            DragOperationDeferral deferral = e.GetDeferral();

            // Get the index the emails were dropped at.
            int index = this.GetInsertionIndex(e);

            // Get the list of emails dropped.
            string emailsString = await e.DataView.GetTextAsync();
            List<Email> emails;
            try
            {
                emails = EmailAdapter.Deserialize(emailsString);
            }
            catch (Exception ex)
            {
                return;
            }

            // Insert each of the emails that were dropped. Insertion is done
            //      in reverse order, because inserting an email at the same
            //      index each time increases the indecies of the previously
            //      inserted emails, resulting in the last email being inserted
            //      appearing on the top of the list.
            List<Email> emailsReversed = emails.ToList();
            emailsReversed.Reverse();
            foreach (Email email in emailsReversed)
            {
                this.Messages.Insert(index, email);
            }

            // Raise the message drop completed event.
            this.RaiseMessageDropCompleted(emails, index, e);

            // Complete the deferral.
            e.AcceptedOperation = DataPackageOperation.Move;
            deferral.Complete();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add an email to be displayed in the message box.
        /// </summary>
        /// <param name="email">The email to add.</param>
        public void AddEmail(Email email)
        {
            this.Messages.Add(email);
        }

        /// <summary>
        /// Removes the given email from the message box, if present.
        /// </summary>
        /// <param name="email">The email to remove.</param>
        public void RemoveEmail(Email email)
        {
            // Find an email in the box with a matching ID.
            Email emailToRemove = this.Messages.Where(e => e.ID == email.ID).FirstOrDefault();

            // Remove the email if a matching email was found.
            if (emailToRemove != null)
            {
                this.Messages.Remove(emailToRemove);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the insertion index on the message list based on the given drag event args.
        /// </summary>
        /// <param name="e">The DrageEventArgs to get relative positions from.</param>
        /// <returns></returns>
        private int GetInsertionIndex(DragEventArgs e)
        {
            // Find the insertion index:
            Point pos = e.GetPosition(this.MessageBoxListView.ItemsPanelRoot);

            // If the target ListView has items in it, use the heigh of the first item
            //      to find the insertion index.
            int index = 0;
            if (this.MessageBoxListView.Items.Count != 0)
            {
                // Get a reference to the first item in the ListView
                ListViewItem sampleItem = (ListViewItem)this.MessageBoxListView.ContainerFromIndex(0);

                // Adjust itemHeight for margins
                double itemHeight = sampleItem.ActualHeight + sampleItem.Margin.Top + sampleItem.Margin.Bottom;

                // Find index based on dividing number of items by height of each item
                index = Math.Min(this.MessageBoxListView.Items.Count - 1, (int)(pos.Y / itemHeight));

                // Find the item being dropped on top of.
                ListViewItem targetItem = (ListViewItem)this.MessageBoxListView.ContainerFromIndex(index);

                // If the drop position is more than half-way down the item being dropped on
                //      top of, increment the insertion index so the dropped item is inserted
                //      below instead of above the item being dropped on top of.
                Point positionInItem = e.GetPosition(targetItem);
                if (positionInItem.Y > itemHeight / 2)
                {
                    index++;
                }

                // Don't go out of bounds
                index = Math.Min(this.MessageBoxListView.Items.Count, index);
            }
            // Only other case is if the target ListView has no items (the dropped item will be
            //      the first). In that case, the insertion index will remain zero.

            return index;
        }
        #endregion
    }
}
