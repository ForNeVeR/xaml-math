using System;

namespace XamlMath.Exceptions;

public sealed class TexParseException : TexException
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
