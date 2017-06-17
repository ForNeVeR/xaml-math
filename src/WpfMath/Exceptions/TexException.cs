using System;
using System.Runtime.Serialization;

namespace WpfMath.Exceptions
{
    public abstract class TexException : Exception
    {
        public TexException()
        {
        }

        public TexException(string message) : base(message)
        {
        }

        public TexException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TexException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
