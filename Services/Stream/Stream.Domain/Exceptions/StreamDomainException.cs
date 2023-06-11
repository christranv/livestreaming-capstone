using System;

namespace Stream.Domain.Exceptions
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class StreamDomainException : Exception
    {
        public StreamDomainException()
        { }

        public StreamDomainException(string message)
            : base(message)
        { }

        public StreamDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
