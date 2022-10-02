using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Email
{
    /// <summary>
    /// Model for an email message.
    /// </summary>
    public class Email
    {
        /// <summary>
        /// The subject of the email.
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// The body of the email.
        /// </summary>
        public string? Body { get; set; }
    }
}
