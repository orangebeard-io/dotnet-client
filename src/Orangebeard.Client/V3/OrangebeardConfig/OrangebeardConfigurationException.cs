using System;
using System.Runtime.Serialization;

namespace Orangebeard.Client.V3.OrangebeardConfig
{
    [Serializable]
    internal class OrangebeardConfigurationException : Exception
    {
        public OrangebeardConfigurationException()
        {
        }

        public OrangebeardConfigurationException(string message) : base(message)
        {
        }

        public OrangebeardConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OrangebeardConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}