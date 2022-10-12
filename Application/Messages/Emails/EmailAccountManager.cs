using Domain.Messages.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages.Emails
{
    /// <summary>
    /// Manages email accounts.
    /// </summary>
    public class EmailAccountManager
    {
        #region Methods
        /// <summary>
        /// Gets a list of emails.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Email>> GetEmailsAsync()
        {
            // Return some test data.
            List<Email> emails = new List<Email>()
            {
                new Email()
                {
                    Subject = "Test Email 1",
                    Body = "Some body text goes here."
                },
                new Email()
                {
                    Subject = "Test Email 2",
                    Body = "This is also body text."
                },
                new Email()
                {
                    Subject = "A MUCH longer email subject that will probably not fit in a smaller space",
                    Body = "Hmm... more body text."
                },
                new Email()
                {
                    Subject = "",
                    Body = "A MUCH longer email body that will probably not fit in a smaller space"
                }
            };

            for (int i = 0; i < 100; i++)
            {
                emails.Add(new Email()
                {
                    Subject = i.ToString() + ". Email Test",
                    Body = "Some body text goes here."
                });
            }

            return emails;
        }
        #endregion
    }
}
