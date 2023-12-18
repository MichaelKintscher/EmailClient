using Application.Persistence;
using Domain.Messages;
using Domain.Messages.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages
{
    /// <summary>
    /// Manages message boxes.
    /// </summary>
    public class MessageBoxManager
    {
        #region Properties
        /// <summary>
        /// A reference to the storage provider this connection manager will interface with.
        /// </summary>
        private IStorageProvider StorageProvider { get; set; }

        /// <summary>
        /// The file name of the file storing the message boxes.
        /// </summary>
        private static readonly string MessageBoxesFileName = "MessageBoxes.json";

        public static MessageBox Inbox
        {
            get
            {
                return new MessageBox()
                {
                    ID = Guid.Empty.ToString(),
                    Name = "Inbox"
                };
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor - takes in a Storage Provider as dependency injection.
        /// </summary>
        /// <param name="storageProvider">The storage provider used for persisting message box data.</param>
        public MessageBoxManager(IStorageProvider storageProvider)
        {
            this.StorageProvider = storageProvider;
        }
        #endregion

        /// <summary>
        /// Creates a new message box with the given name.
        /// </summary>
        /// <param name="name">The name to give the new message box.</param>
        /// <returns>The newly created message box.</returns>
        #region Methods
        public async Task<MessageBox> CreateMessageBoxAsync(string name)
        {
            // Create a new message box.
            MessageBox messageBox = MessageBoxFactory.CreateNewBox(name);

            // Append the new message box to the existing list of message boxes and then save.
            List<MessageBox> messageBoxes = await this.StorageProvider.LoadAsync<MessageBox>(MessageBoxManager.MessageBoxesFileName);
            messageBoxes.Add(messageBox);
            await this.StorageProvider.SaveAsync<MessageBox>(messageBoxes, MessageBoxManager.MessageBoxesFileName);

            // Return the new message box.
            return messageBox;
        }

        /// <summary>
        /// Gets a list of all message boxes, without populating the message list in the boxes.
        /// </summary>
        /// <returns></returns>
        public async Task<List<MessageBox>> GetMessageBoxesAsync()
        {
            // Get the message boxes.
            List<MessageBox> messageBoxes = await this.StorageProvider.LoadAsync<MessageBox>(MessageBoxManager.MessageBoxesFileName);
            return messageBoxes;
        }

        /// <summary>
        /// Renames the given message box with the given name.
        /// </summary>
        /// <param name="messageBoxId">The ID of the message box to rename.</param>
        /// <param name="name">The new name to give the message box.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task RenameMessageBoxAsync(string messageBoxId, string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Moves the given emails to the message box matching the given ID.
        /// </summary>
        /// <param name="emails">The list of emails to move to the message box.</param>
        /// <param name="messageBoxId">The ID of the message box to move the emails to.</param>
        /// <exception cref="ArgumentException">Thrown if a given email is not found among the saved emails.</exception>
        public async Task MoveEmailsToMessageBoxAsync(List<Email> emails, string messageBoxId)
        {
            // Load the saved messages.
            List<Email> savedEmails = await this.StorageProvider.LoadAsync<Email>(MessagesManager.MessgesFileName);

            // For each email to move...
            foreach (Email email in emails)
            {
                // Get a reference to the email from the saved messages.
                Email? savedEmail = savedEmails.Where(e => e.ID == email.ID).FirstOrDefault();
                if (savedEmail == null)
                {
                    throw new ArgumentException("Email to move was not found among the saved emails.");
                }

                // Update the saved email's message box assignment.
                savedEmail.MessageBoxID = messageBoxId;
            }

            // Save the updated messages.
            await this.StorageProvider.SaveAsync<Email>(savedEmails, MessagesManager.MessgesFileName);
        }

        /// <summary>
        /// Deletes the given message box.
        /// </summary>
        /// <param name="messageBoxId">The ID of the message box to delete.</param>
        /// <returns>Whether the given message box was successfully deleted.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> DeleteMessageBoxAsync(string messageBoxId)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
