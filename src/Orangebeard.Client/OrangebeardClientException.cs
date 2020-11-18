using System;

namespace Orangebeard.Client
{
    /// <summary>
    /// Occurs when a request could not be processed.
    /// </summary>
    public class OrangebeardClientException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OrangebeardClientException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="cause">Reference to the exception causing this error.</param>
        public OrangebeardClientException(string message, Exception cause) : base(message, cause)
        {

        }
    }
}
