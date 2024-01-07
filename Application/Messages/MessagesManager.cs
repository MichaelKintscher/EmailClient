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
        /// A reference to the storage provider this messages manager will interface with.
        /// </summary>
        private IStorageProvider StorageProvider { get; set; }

        /// <summary>
        /// A reference to the message service provider this messages manager will interface with. 
        /// </summary>
        private IMessageService MessageService { get; set; }

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
        /// 
        /// <param name="messageService">The message service used for retrieving message data.</param>
        /// <param name="storageProvider">The storage provider used for persisting message data.</param>
        public MessagesManager(IMessageService messageService, IStorageProvider storageProvider)
        {
            this.StorageProvider = storageProvider;
            this.MessageService = messageService;
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

        /// <summary>
        /// Gets a list of messages in the the inbox on the server.
        /// </summary>
        /// <returns></returns>
        public Task<List<Email>> GetInboxMessagesFromServerAsync(string accountId)
        {
            return this.MessageService.GetMessagesAsync(accountId);
        }
        #endregion
    }
}
