using Domain.Common;
using Domain.Messages;
using Domain.Messages.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Config
{
    public interface IStorageProvider
    {
        #region Methods - Messages
        public Task SaveMessagesAsync(string messagesFileName, List<Email> emails);

        public Task<List<Email>> LoadMessagesAsync(string messagesFileName);
        #endregion

        #region Methods - Message Boxes
        public Task SaveMessageBoxesAsync(string messageBoxesFileName, List<MessageBox> messageBoxes);

        public Task<List<MessageBox>> LoadMessageBoxesAsync(string messageBoxesFileName);
        #endregion

        #region Methods - OAuth
        public Task SaveConnectedAccountsAsync(string accountsFileName, List<ServiceProviderAccount> accounts);

        public Task<List<ServiceProviderAccount>> LoadConnectedAccountsAsync(string accountsFileName);

        public Task SaveConnectionDataAsync(string tokenFileName, Dictionary<string, OAuthToken> tokens);

        public Task<Dictionary<string, OAuthToken>> TryLoadTokenDataAsync(string tokenFileName);
        #endregion
    }
}
