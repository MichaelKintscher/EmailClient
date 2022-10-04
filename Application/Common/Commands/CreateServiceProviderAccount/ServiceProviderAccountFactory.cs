using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Commands.CreateServiceProviderAccount
{
    /// <summary>
    /// Factory class for creating service provider accounts.
    /// </summary>
    public static class ServiceProviderAccountFactory
    {
        /// <summary>
        /// Creates a new service provider account with the given parameters.
        /// </summary>
        /// <param name="accountId">The unique ID of the account given locally by the app.</param>
        /// <param name="providerName">The name of the service provider.</param>
        /// <param name="providerId">The unique ID of the account with the service provider.</param>
        /// <param name="userName">The username for the account with the service provider.</param>
        /// <param name="pictureUri">The uri for the profile picture online.</param>
        /// <param name="connected">Whether the account is connected.</param>
        /// <returns></returns>
        public static ServiceProviderAccount CreateServiceProviderAccount(string accountId, string providerName, string providerId, string userName, string pictureUri, bool connected)
        {
            ServiceProviderAccount account = new ServiceProviderAccount()
            {
                ID = accountId,
                Provider = providerName,
                ProviderGivenID = providerId,
                FriendlyName = "Test Account",
                Username = userName,
                PictureUri = pictureUri,
                PictureLocalUri = "",
                Connected = connected,
                LastSynced = DateTime.Now
            };

            return account;
        }
    }
}
