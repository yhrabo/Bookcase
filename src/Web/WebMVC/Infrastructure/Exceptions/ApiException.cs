using System;
using System.Net;
using System.Runtime.Serialization;

namespace WebMVC.Infrastructure.Exceptions
{
    /// <summary>
    /// Represents an error received as a response from API.
    /// </summary>
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public ApiException()
        {
        }

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(HttpStatusCode code, string message) : base(message)
        {
            StatusCode = code;
        }

        public ApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
