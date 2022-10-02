using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsOS.Persistence.Exceptions
{
    /// <summary>
    /// Thrown when an error occurs while interfacing with Windows Storage.
    /// </summary>
    internal class WindowsStorageException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the WindowsStorageException class with the given error message.
        /// </summary>
        /// <param name="message">The error message for the exception.</param>
        public WindowsStorageException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the WindowsStorageException class with the given error message and inner exception.
        /// </summary>
        /// <param name="message">The error message for the exception.</param>
        /// <param name="innerException">A reference to the inner exception causing this exception.</param>
        public WindowsStorageException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
