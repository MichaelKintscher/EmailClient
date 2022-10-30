using Application.Config;
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
            List<MessageBox> messageBoxes = await this.StorageProvider.LoadMessageBoxesAsync(MessageBoxManager.MessageBoxesFileName);
            messageBoxes.Add(messageBox);
            await this.StorageProvider.SaveMessageBoxesAsync(MessageBoxManager.MessageBoxesFileName, messageBoxes);

            // Return the new message box.
            return messageBox;
        }

        /// <summary>
        /// Gets a list of all message boxes.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<List<MessageBox>> GetMessageBoxesAsync()
        {
            return Task.FromResult(new List<MessageBox>()
            {
                new MessageBox()
                {
                    Name = "Second Box",
                },
                new MessageBox()
                {
                    Name = "Third Box",
                },
                new MessageBox()
                {
                    Name = "Fourth Box",
                },
                new MessageBox()
                {
                    Name = "Fifth Box",
                },
                new MessageBox()
                {
                    Name = "Sixth Box",
                }
            });
        }

        /// <summary>
        /// Renames the given message box with the given name.
        /// </summary>
        /// <param name="messageBoxId">The ID of the message box to rename.</param>
        /// <param name="name">The new name to give the message box.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task RenameMessageBoxAsync(string messageBoxId, string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the given email to the message box matching the given ID.
        /// </summary>
        /// <param name="email">The email to add to the message box.</param>
        /// <param name="messageBoxId">The ID of the message box to add the email to.</param>
        /// <returns>Whether the given email was successfully added to the given message box.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> AddEmailToMessageBoxAsync(Email email, string messageBoxId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the given email from the message box matching the given ID.
        /// </summary>
        /// <param name="emailId">The app-given ID of the email to remove from the message box.</param>
        /// <param name="messageBoxId">The ID of the message box to remove the email from.</param>
        /// <returns>Whether the given email was successfully removed from the given message box.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> RemoveEmailFromMessageBox(string emailId, string messageBoxId)
        {
            throw new NotImplementedException();
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
