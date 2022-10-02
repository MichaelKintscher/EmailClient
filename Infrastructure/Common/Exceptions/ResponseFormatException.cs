using InterfaceAdapters.Json.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Common.Exceptions
{
    /// <summary>
    /// Thrown when a JSON response is not in the expected format.
    /// </summary>
    public class ResponseFormatException : JsonFormatException
    {
        /// <summary>
        /// Initializes a new instance of the ResponseFormatException class with the given error message.
        /// </summary>
        /// <param name="message">The error message for the exception.</param>
        public ResponseFormatException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the ResponseFormatException class with the given error message and inner exception.
        /// </summary>
        /// <param name="message">The error message for the exception.</param>
        /// <param name="innerException">A reference to the inner exception causing this exception.</param>
        public ResponseFormatException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
