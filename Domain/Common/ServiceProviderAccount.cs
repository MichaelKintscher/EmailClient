using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    /// <summary>
    /// Represents an account with a service provider.
    /// </summary>
    public class ServiceProviderAccount
    {
        /// <summary>
        /// The unique ID of the account given locally by the app.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The service provider's name.
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// The unique ID of the account with the service provider.
        /// </summary>
        public string ProviderGivenID { get; set; }

        /// <summary>
        /// The name for the account given by the user. Must be unique.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// The username for the account with the service provider.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The uri for the profile picture online.
        /// </summary>
        public string PictureUri { get; set; }

        /// <summary>
        /// The uri for the cached profile picture. Use this is a fallback if PictureUri is inacessible.
        /// </summary>
        public string PictureLocalUri { get; set; }

        /// <summary>
        /// Whether the account is connected.
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// When data from this account was last synced.
        /// </summary>
        public DateTime LastSynced { get; set; }

        /// <summary>
        /// Default constructor - creates an instance with the connected flag as false, the last synced datetime to a minimum value, and all other properties to empty strings.
        /// </summary>
        public ServiceProviderAccount()
        {
            this.ID = string.Empty;
            this.Provider = string.Empty;
            this.ProviderGivenID = string.Empty;
            this.FriendlyName = string.Empty;
            this.Username = string.Empty;
            this.PictureUri = string.Empty;
            this.PictureLocalUri = string.Empty;
            this.Connected = false;

            // The minimum value ensures that *any* current datetime will be after the default value.
            this.LastSynced = DateTime.MinValue;
        }
    }
}
