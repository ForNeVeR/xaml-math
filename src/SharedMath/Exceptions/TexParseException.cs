using System;

namespace WpfMath.Exceptions
{
    public class TexParseException : TexException
    {
        internal TexParseException(string message)
            : base(message)
        {
        }

        internal TexParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
