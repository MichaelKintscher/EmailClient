using Application.Config;
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
        /// A reference to the service provider account this message manager manages messages for.
        /// </summary>
        private ServiceProviderAccount ServiceProviderAccount { get; set; }

        /// <summary>
        /// A reference to the message service this message manager manages.
        /// </summary>
        private IMessageService MessageService { get; set; }

        /// <summary>
        /// A reference to the storage provider this connection manager will interface with.
        /// </summary>
        private IStorageProvider StorageProvider { get; set; }

        /// <summary>
        /// The file name of the file storing the messages for the account and message service this message manager manages.
        /// </summary>
        private string MessgesFileName
        {
            get => this.ServiceProviderAccount.ID + "_" + this.MessageService.Name + "_messages.json";
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor - takes in a Message Service Provider and Storage Provider as dependency injection.
        /// </summary>
        /// <param name="serviceProviderAccount">The service provider account the messages are associated with.</param>
        /// <param name="messageService">The message service provider the messages are associated with.</param>
        /// <param name="storageProvider">The storage provider used for persisting message data.</param>
        public MessagesManager(ServiceProviderAccount serviceProviderAccount, IMessageService messageService, IStorageProvider storageProvider)
        {
            this.ServiceProviderAccount = serviceProviderAccount;
            this.MessageService = messageService;
            this.StorageProvider = storageProvider;
        }
        #endregion

        #region Methods
        public Task<List<Email>> GetMessagesAsync()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
