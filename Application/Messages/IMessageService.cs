using Domain.Messages.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages
{
    public interface IMessageService
    {
        /// <summary>
        /// The name of the API.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the messages from the given email provider account.
        /// </summary>
        /// <param name="accountId">The ID for the account assigned by the app.</param>
        /// <returns></returns>
        public Task<List<Email>> GetMessagesAsync(string accountId);
    }
}
