using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InterfaceAdapters.Json.Exceptions
{
    /// <summary>
    /// Thrown when a JSON object is not in the expected format.
    /// </summary>
    public class JsonFormatException : JsonException
    {
        /// <summary>
        /// Initializes a new instance of the ResponseFormatException class with the given error message.
        /// </summary>
        /// <param name="message">The error message for the exception.</param>
        public JsonFormatException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the ResponseFormatException class with the given error message and inner exception.
        /// </summary>
        /// <param name="message">The error message for the exception.</param>
        /// <param name="innerException">A reference to the inner exception causing this exception.</param>
        public JsonFormatException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
