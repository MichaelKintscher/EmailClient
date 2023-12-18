using Application.Persistence;
using Domain.Common;
using Domain.Messages.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages
{
    /// <summary>
    /// Manages the messages for a specific message service.
    /// </summary>
    public class MessagesManager
    {
        #region Properties
        /// <summary>
        /// A reference to the storage provider this connection manager will interface with.
        /// </summary>
        private IStorageProvider StorageProvider { get; set; }

        /// <summary>
        /// The file name of the file storing the messages for the account and message service this message manager manages.
        /// </summary>
        public static string MessgesFileName
        {
            get => "messages.json";
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor - takes in a Message Service Provider and Storage Provider as dependency injection.
        /// </summary>
        /// <param name="storageProvider">The storage provider used for persisting message data.</param>
        public MessagesManager(IStorageProvider storageProvider)
        {
            this.StorageProvider = storageProvider;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets a list of all saved messages.
        /// </summary>
        /// <returns></returns>
        public Task<List<Email>> GetAllMessagesAsync()
        {
            return this.StorageProvider.LoadAsync<Email>(MessagesManager.MessgesFileName);
        }
        #endregion
    }
}
