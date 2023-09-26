using System;

namespace XamlMath;

public sealed class SymbolNotFoundException : Exception
{
    internal SymbolNotFoundException(string symbolName)
        : base($"Cannot find symbol with the name '{symbolName}'.")
    {
    }
}
