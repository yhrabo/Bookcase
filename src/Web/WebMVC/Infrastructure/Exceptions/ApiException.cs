using System;
using System.Runtime.Serialization;

namespace WebMVC.Infrastructure.Exceptions
{
    /// <summary>
    /// Represents an error received as a response from API.
    /// </summary>
    public class ApiException : Exception
    {
        public ApiException()
        {
        }

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
