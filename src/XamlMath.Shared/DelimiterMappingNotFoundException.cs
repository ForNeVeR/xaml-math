using System;

namespace XamlMath;

public sealed class DelimiterMappingNotFoundException : Exception
{
    internal DelimiterMappingNotFoundException(char delimiter)
        : base($"Cannot find delimeter mapping for the character '{delimiter}'.")
    {
    }
}
