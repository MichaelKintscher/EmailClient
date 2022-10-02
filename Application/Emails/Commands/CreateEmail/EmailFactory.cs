using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Emails;

namespace Application.Emails.Commands.CreateEmail
{
    /// <summary>
    /// Factory class for creating emails.
    /// </summary>
    public class EmailFactory
    {
        /// <summary>
        /// Creates a new email with the given subject and body.
        /// </summary>
        /// <param name="subject">The subject to give the email.</param>
        /// <param name="body">The body to give the email.</param>
        /// <returns>The created email.</returns>
        public Email Create(string subject, string body)
        {
            // Create a new instance of the email class.
            Email email = new Email
            {
                // Assign the subject and body properties from the input parameters.
                Subject = subject,
                Body = body
            };

            // Return the new email instance.
            return email;
        }
    }
}
