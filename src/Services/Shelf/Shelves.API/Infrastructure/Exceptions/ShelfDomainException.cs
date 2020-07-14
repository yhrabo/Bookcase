using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Bookcase.Services.Shelves.API.Infrastructure.Exceptions
{
    public class ShelfDomainException : Exception
    {
        public ShelfDomainException()
        {
        }

        public ShelfDomainException(string message) : base(message)
        {
        }

        public ShelfDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
