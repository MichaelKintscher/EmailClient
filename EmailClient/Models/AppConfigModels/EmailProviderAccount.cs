using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailClient.Models.AppConfigModels
{
    /// <summary>
    /// Represents an account with a email service provider.
    /// </summary>
    public class EmailProviderAccount
    {
        /// <summary>
        /// The unique ID of the account given locally by the app.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The email service providers.
        /// </summary>
        public EmailProvider Provider { get; set; }

        /// <summary>
        /// The unique ID of the account with the service provider.
        /// </summary>
        public string ProviderGivenID { get; set; }

        /// <summary>
        /// The name for the account given by the user. Must be unique.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// The username for the account with the email service provider.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The uri for the profile picture online.
        /// </summary>
        public string PictureUri { get; set; }

        /// <summary>
        /// The uri for the cahced profile picture. Use this is a fallback if PictureUri is inacessible.
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
    }

    /// <summary>
    /// Represents email providers supported by the app.
    /// </summary>
    public enum EmailProvider
    {
        Google
    }
}
